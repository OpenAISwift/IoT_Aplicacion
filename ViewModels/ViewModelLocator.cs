using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;


namespace IoT_Aplicacion_UWP.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<Nodo1ViewModel>();
            //SimpleIoc.Default.Register<ACViewModel>();
        }

        public Nodo1ViewModel N1
        {
            get
            {
                return ServiceLocator.Current.GetInstance<Nodo1ViewModel>();
            }
        }
    }
}
