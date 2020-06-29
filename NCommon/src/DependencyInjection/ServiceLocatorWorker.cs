using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CommonServiceLocator;

namespace NCommon.DependencyInjection
{
    /// <summary>
    /// This is a facade for the <see cref="ServiceLocator">ServiceLocator</see>/> class. It handles all of the validation and 
    /// generates specific Exceptions you can use to debug IOC errors more effectively.
    /// </summary>
    /// <exception cref="ServiceLocationException">ServiceLocationException</exception>
    public static class ServiceLocatorWorker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static TService GetInstance<TService>(string keyName)
        {
            try
            {
                if (string.IsNullOrEmpty(keyName))
                {
                    throw new ArgumentNullException(keyName, "keyName cannot be empty or null when attempting to acquire Service from an IOC Container.");
                }
                TService serv = GetInstance<TService>(ServiceLocator.Current, keyName);
                if (serv != null)
                {
                    return serv;
                }
                else
                {
                    throw new ApplicationException("Sevice returned from IOC Container was null.");
                }
            }
            catch (ArgumentNullException ex) // keyName is empty
            {
                throw new ServiceLocationException(ex.Message, typeof(TService), ex);
            }
            catch (ActivationException ex)
            {

                throw new ServiceLocationException(ex.Message, typeof(TService), keyName, ex);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService GetInstance<TService>()
        {
            try
            {

                TService serv = GetInstance<TService>(ServiceLocator.Current);
                if (serv != null)
                {
                    return serv;
                }
                else
                {
                    throw new ApplicationException("Sevice returned from IOC Container was null.");
                }
            }
            catch (ArgumentNullException ex) // keyName is empty
            {
                throw new ServiceLocationException(ex.Message, typeof(TService), ex);
            }
            catch (ActivationException ex)
            {

                throw new ServiceLocationException(ex.Message, typeof(TService), "No Key Defined", ex);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetInstance(Type type)
        {
            try
            {

                var serv = GetInstance(ServiceLocator.Current, type);

                if (serv != null)
                {
                    return serv;
                }
                else
                {
                    throw new ApplicationException("Sevice returned from IOC Container was null.");
                }
            }
            catch (ArgumentNullException ex) // keyName is empty
            {
                throw new ServiceLocationException(ex.Message, type.GetType(), ex);
            }
            catch (ActivationException ex)
            {

                throw new ServiceLocationException(ex.Message, type.GetType(), "No Key Defined", ex);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetInstance(IServiceLocator locator, Type type)
        {
            try
            {
                if (locator == null)
                {
                    //throw new ServiceLocationException("The current IServiceLocator could not be found prior to loading the IOC container from key: " + keyName);
                    throw new ArgumentNullException("locator", "The IServiceLocator cannot be null when attempting to acquire Service from an IOC Container."
                        + " It may not have been set during application initialization.");
                }

                var serv = locator.GetInstance(type);
                if (serv != null)
                {
                    return serv;
                }
                else
                {
                    throw new ApplicationException("Sevice returned from IOC Container was null.");
                }
            }
            catch (ArgumentNullException ex) // keyName is empty
            {
                throw new ServiceLocationException(ex.Message, type.GetType(), ex);
            }
            catch (ServiceLocationException ex)
            {

                throw ex;
            }
            catch (ActivationException ex)
            {
                throw new ServiceLocationException(ex.Message, type.GetType(), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="locator"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static TService GetInstance<TService>(IServiceLocator locator, string keyName)
        {
            try
            {
                if (locator == null)
                {
                    //throw new ServiceLocationException("The current IServiceLocator could not be found prior to loading the IOC container from key: " + keyName);
                    throw new ArgumentNullException("locator", "The IServiceLocator cannot be null when attempting to acquire Service from an IOC Container."
                        + " It may not have been set during application initialization.");
                }
                if (string.IsNullOrEmpty(keyName))
                {
                    throw new ArgumentNullException(keyName, "keyName cannot be empty or null when attempting to acquire Service from an IOC Container.");
                }
                TService serv = locator.GetInstance<TService>(keyName);
                if (serv != null)
                {
                    return serv;
                }
                else
                {
                    throw new ApplicationException("Sevice returned from IOC Container was null.");
                }
            }
            catch (ActivationException ex)
            {
                throw new ServiceLocationException(ex.Message, typeof(TService), ex);
            }
            catch (ArgumentNullException ex) // keyName is empty
            {
                throw new ServiceLocationException(ex.Message, typeof(TService), ex);
            }
            catch (ServiceLocationException ex)
            {

                throw ex;
            }
            catch (ApplicationException ex)
            {
                throw new ServiceLocationException(ex.Message, typeof(TService), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static TService GetInstance<TService>(IServiceLocator locator)
        {
            try
            {
                if (locator == null)
                {
                    //throw new ServiceLocationException("The current IServiceLocator could not be found prior to loading the IOC container from key: " + keyName);
                    throw new ArgumentNullException("locator", "The IServiceLocator cannot be null when attempting to acquire Service from an IOC Container."
                        + " It may not have been set during application initialization.");
                }

                TService serv = locator.GetInstance<TService>();
                if (serv != null)
                {
                    return serv;
                }
                else
                {
                    throw new ApplicationException("Sevice returned from IOC Container was null.");
                }
            }
            catch (ArgumentNullException ex) // keyName is empty
            {
                throw new ServiceLocationException(ex.Message, typeof(TService), ex);
            }
            catch (ServiceLocationException ex)
            {

                throw ex;
            }
            catch (ApplicationException ex)
            {
                throw new ServiceLocationException(ex.Message, typeof(TService), ex);
            }
            catch (ActivationException ex)
            {
                throw new ServiceLocationException(ex.Message, typeof(TService), ex);
            }
        }


        public static IEnumerable<TService> GetAllInstances<TService>()
        {
            try
            {

                var serv = GetAllInstances<TService>(ServiceLocator.Current);
                if (serv != null)
                {
                    return serv;
                }
                else
                {
                    throw new ApplicationException("Sevice returned from IOC Container was null.");
                }
            }
            catch (ArgumentNullException ex) // keyName is empty
            {
                throw new ServiceLocationException(ex.Message, typeof(TService), ex);
            }
            catch (ActivationException ex)
            {

                throw new ServiceLocationException(ex.Message, typeof(TService), "No Key Defined", ex);
            }

        }

        public static IEnumerable<TService> GetAllInstances<TService>(IServiceLocator locator)
        {
            try
            {
                if (locator == null)
                {
                    //throw new ServiceLocationException("The current IServiceLocator could not be found prior to loading the IOC container from key: " + keyName);
                    throw new ArgumentNullException("locator", "The IServiceLocator cannot be null when attempting to acquire Service from an IOC Container."
                        + " It may not have been set during application initialization.");
                }

                var serv = locator.GetAllInstances<TService>();
                if (serv != null)
                {
                    return serv;
                }
                else
                {
                    throw new ApplicationException("Sevice returned from IOC Container was null.");
                }
            }
            catch (ArgumentNullException ex)
            {
                throw new ServiceLocationException(ex.Message, typeof(TService), ex);
            }
            catch (ServiceLocationException ex)
            {

                throw ex;
            }
            catch (ActivationException ex)
            {
                throw new ServiceLocationException(ex.Message, typeof(TService), ex);
            }
        }

        public static IEnumerable<object> GetAllInstances(IServiceLocator locator, Type type)
        {
            try
            {
                if (locator == null)
                {
                    //throw new ServiceLocationException("The current IServiceLocator could not be found prior to loading the IOC container from key: " + keyName);
                    throw new ArgumentNullException("locator", "The IServiceLocator cannot be null when attempting to acquire Service from an IOC Container."
                        + " It may not have been set during application initialization.");
                }

                var serv = locator.GetAllInstances(type);
                if (serv != null)
                {
                    return serv;
                }
                else
                {
                    throw new ApplicationException("Sevice returned from IOC Container was null.");
                }
            }
            catch (ArgumentNullException ex)
            {
                throw new ServiceLocationException(ex.Message, type, ex);
            }
            catch (ServiceLocationException ex)
            {

                throw ex;
            }
            catch (ActivationException ex)
            {
                throw new ServiceLocationException(ex.Message, type, ex);
            }
        }

        public static IEnumerable<object> GetAllInstances(Type type)
        {
            try
            {

                var serv = GetAllInstances(ServiceLocator.Current, type);
                if (serv != null)
                {
                    return serv;
                }
                else
                {
                    throw new ApplicationException("Sevice returned from IOC Container was null.");
                }
            }
            catch (ArgumentNullException ex)
            {
                throw new ServiceLocationException(ex.Message, type, ex);
            }
            catch (ServiceLocationException ex)
            {

                throw ex;
            }
            catch (ActivationException ex)
            {
                throw new ServiceLocationException(ex.Message, type, ex);
            }
        }

    }
}
