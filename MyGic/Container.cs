using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyGic
{
    public class Container : IContainer
    {
        ConcurrentDictionary<string, InstanceType> instanceCollection = new ConcurrentDictionary<string, InstanceType>();
        public void Register<I, C>(LifeCycleType lifeCycleType = LifeCycleType.Transient)
            where I : class
            where C : class
        {
            if (!typeof(I).IsAssignableFrom(typeof(C)))
            {
                throw new InvalidCastException("Cannot be assignable of this type.");
            }

            if (instanceCollection.ContainsKey(typeof(I).FullName))
                throw new Exception($"Duplicate registration of type {typeof(I).FullName}");

            instanceCollection.TryAdd(typeof(I).FullName, new InstanceType
            {
                InterfaceType = typeof(I),
                ImplementingType = typeof(C),
                LifeCyle = lifeCycleType
            });
        }


        public T Resolve<T>()
        {
            try
            {
                var typeFullName = typeof(T).FullName;

                var isExists = instanceCollection.TryGetValue(typeFullName, out InstanceType resolvedType);

                if (!isExists)
                    throw new Exception($"The type {typeFullName} is not registered");

                if (HasRecyclicDependency(typeof(T)))
                    throw new Exception($"A circular dependency was detected for the service of type '{typeFullName}'");

                switch (resolvedType.LifeCyle)
                {
                    case LifeCycleType.Singleton:
                        return resolvedType.Instance == null ? (T)CreateSington<T>() : (T) resolvedType.Instance;
                    case LifeCycleType.Transient:
                        return (T)CreateTransient(resolvedType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return default(T);
        }      

        private object CreateSington<T>()
        {
            string typeFullName = typeof(T).FullName;
            var isInstanceExists = instanceCollection.TryGetValue(typeFullName, out InstanceType instanceType);

            if (!isInstanceExists) throw new Exception($"The type of {typeFullName} is not registered.");

            if (instanceType.Instance != null)
                return instanceType.Instance;

            List<object> typeParams = new List<object>();

            ConstructorInfo[] constructorInfos = instanceType.ImplementingType.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (var consInfo in constructorInfos)
            {
                var parameterInfos = consInfo.GetParameters();

                foreach (var param in parameterInfos)
                {
                    var typeParam = param.ParameterType;
                    var typeName = typeParam.FullName;

                    bool isExists = instanceCollection.TryGetValue(typeName, out InstanceType instanceParamType);

                    if (!isExists)
                        throw new Exception($"This type {typeName} is not registered.");

                    if (instanceParamType.LifeCyle == LifeCycleType.Singleton)
                    {
                        if (instanceParamType.Instance == null)
                        {
                            var objType = Activator.CreateInstance(instanceParamType.ImplementingType);
                            instanceParamType.Instance = objType;
                            typeParams.Add(objType);
                        }
                        else
                        {
                            typeParams.Add(instanceParamType.Instance);
                        }

                    }
                    else if (instanceParamType.Instance == null)
                    {
                        var objType = Activator.CreateInstance(instanceParamType.ImplementingType);                      
                        typeParams.Add(objType);
                    }
                }
            }

            if (instanceType.Instance == null)
            {
                instanceType.Instance = Activator.CreateInstance(instanceType.ImplementingType, typeParams.ToArray());
            }                                         

            return instanceType.Instance;
        }


        //private object CreateSingletonR<I>()
        //{
        //    string interfaceTypeName = typeof(I).FullName;

        //    var isRegistered = instanceCollection.TryGetValue(interfaceTypeName, out InstanceType instanceType);

        //    if (!isRegistered)
        //        throw new Exception($"The type of {interfaceTypeName} is not registered.");

        //    if (instanceType.Instance != null)
        //        return instanceType.Instance;

        //    List<object> listParam = new List<object>();
        //    bool isStillHasParam = true;

        //    int ctr = 0;
        //    Dictionary<int, List<object>> ctrParams = new Dictionary<int, List<object>>();

        //    do
        //    {
        //        var typeConstructorInfo = instanceType.ImplementingType.GetConstructors();

        //        if (typeConstructorInfo.Any())
        //        {
        //            List<object> paramObject = new List<object>();

        //            foreach(var contInfo in typeConstructorInfo)
        //            {
        //                var paramObjecInfos = contInfo.GetParameters();

        //                foreach(var paramO in paramObjecInfos)
        //                {
        //                    var paramOType = paramO.ParameterType;
        //                    var paramOTypeName = paramOType.FullName;

        //                    paramObject.Add()
        //                }
        //            }


        //            ctr++;
        //        }
        //        else
        //            isStillHasParam = false;

        //    } while (isStillHasParam);
        //}



        private object CreateTransient(InstanceType instanceType)
        {

            string typeFullName = instanceType.InterfaceType.FullName;

            if (instanceType == null)
                throw new Exception($"The type of {typeFullName} is not registered.");

            List<object> typeParams = new List<object>();
            Type concreteType = instanceType.ImplementingType;


            ConstructorInfo[] constructorInfos = concreteType.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            if (constructorInfos.Any())
            {
                foreach (var consInfo in constructorInfos)
                {
                    var parameterInfos = consInfo.GetParameters();

                    if (parameterInfos.Any())
                    {
                        foreach (var param in parameterInfos)
                        {
                            var typeParam = param.ParameterType;
                            var typeParamName = typeParam.FullName;

                            var isInstanceExists = instanceCollection.TryGetValue(typeParamName, out InstanceType instanceParam);

                            if (!isInstanceExists)
                                throw new Exception($"The type of {typeFullName} is not registered.");

                            if (instanceParam.LifeCyle == LifeCycleType.Transient)
                            {
                                typeParams.Add(Activator.CreateInstance(instanceParam.ImplementingType));
                            }
                            else
                            {
                                if (instanceParam.Instance == null)
                                {
                                    instanceParam.Instance = Activator.CreateInstance(instanceParam.ImplementingType);                                    
                                }
                                typeParams.Add(instanceParam.Instance);
                            }
                        }
                    }
                }
            }
            return Activator.CreateInstance(instanceType.ImplementingType, typeParams.ToArray());
        }

        private bool HasRecyclicDependency(Type interfaceType)
        {
            bool result = false;
            string interfaceFullName = interfaceType.FullName;
            InstanceType instanceType = instanceCollection[interfaceFullName];

            if (instanceType != null)
            {
                var injectedConstructorInfos = instanceType.ImplementingType.GetConstructors();

                if (injectedConstructorInfos.Any())
                {
                    foreach (var constructorInfo in injectedConstructorInfos)
                    {
                        ParameterInfo[] injectedParamInfos = constructorInfo.GetParameters();

                        foreach (ParameterInfo paramInfo in injectedParamInfos)
                        {
                            Type pInfo = paramInfo.ParameterType;
                            string pInfoName = pInfo.FullName;

                            bool isExists = instanceCollection.TryGetValue(pInfoName, out InstanceType injectedParameter);

                            if (isExists)
                            {
                                var pinfoParamType = injectedParameter.ImplementingType.GetConstructors();

                                foreach (var pInfoParam in pinfoParamType)
                                {
                                    ParameterInfo[] pInfoParamTypeInfos = pInfoParam.GetParameters();

                                    foreach (var pTypeInfo in pInfoParamTypeInfos)
                                    {
                                        Type parameterType = pTypeInfo.ParameterType;
                                        string parameterTypeName = parameterType.FullName;

                                        bool isInjectedExists = instanceCollection.TryGetValue(parameterTypeName, out InstanceType injectedParamParamType);

                                        if (isInjectedExists)
                                        {
                                            string injectedParamType = injectedParamParamType.ImplementingType.FullName;

                                            if (instanceType.ImplementingType.FullName.CompareTo(injectedParamType) == 0)
                                                return true;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }

            return result;

        }
    }
}
