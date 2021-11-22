using GalaSoft.MvvmLight;
using ThingSpeakWinRT;
using IoT_Aplicacion_UWP.Utils;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace IoT_Aplicacion_UWP.ViewModels
{
    public class Nodo1ViewModel : ViewModelBase
    {
        private const string char_StartMarker = "<";
        private const string char_EndMarker = ">";

        private readonly string[] str_Delimiter = new string[] { " ", ",", ":" };
        public string[] str_data;

        private string str_ReadString = "0";
        private string str_TimeReadString = "0";
        private string str_Humedad = "0";
        private string str_Temperatura = "0";
        private string str_Temperatura1 = "0";
        private string str_Temperatura2 = "0";
        private string str_Ldr = "0";
        private string str_PAtmosferica = "0";
        private string str_Altitud = "0";
        private string str_Intervalo = "0";

        public RelayCommand RelayC_LED1 { get; private set; }
        private bool bool_ChekedLed1;
        public RelayCommand RelayC_LED2 { get; private set; }
        private bool bool_ChekedLed2;
        public RelayCommand RelayC_LEDAll { get; private set; }
        private bool bool_ChekedLedAll;

        public RelayCommand RelayC_Actualizar { get; private set; }

        private readonly SerialPortService portService;

        public Nodo1ViewModel()
        {
            portService = new SerialPortService();

            RelayC_LED1 = new RelayCommand(Led1Async);
            RelayC_LED2 = new RelayCommand(Led2Async);
            RelayC_LEDAll = new RelayCommand(LedAllAsync);
            RelayC_Actualizar = new RelayCommand(ActualizarAsync);

            portService.Event_Data += PortService_Event_Data;
        }

        private void PortService_Event_Data(object sender, EventArgs e)
        {
            string str_dataInt = (string)sender;
            TimerReadString = DateTime.Now.ToString(System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.LongTimePattern);
            ReadString = str_dataInt;

            str_data = str_dataInt.Split(str_Delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (str_data[0] == char_StartMarker)
            {
                switch (str_data[1])
                {
                    case "N1":
                        if (str_data[2] == "1")
                        {
                            for (byte i = 1; i < str_data.Length; i += 1)
                            {
                                switch (str_data[i])
                                {
                                    case "HD":
                                        Humedad = Convert.ToString(Int64.Parse(str_data[i + 1]));
                                        break;

                                    case "TD":
                                        Temperatura = Convert.ToString(Int64.Parse(str_data[i + 1]));
                                        break;

                                    case "T1":
                                        Temperatura1 = Convert.ToString(Int64.Parse(str_data[i + 1]));
                                        break;

                                    case "T2":
                                        Temperatura2 = Convert.ToString(Int64.Parse(str_data[i + 1]));
                                        break;

                                    case "LD":
                                        LDR = Convert.ToString(Int64.Parse(str_data[i + 1]));
                                        break;

                                    case "PR":
                                        PAtmos = Convert.ToString(Int64.Parse(str_data[i + 1]));
                                        break;

                                    case "AL":
                                        Altitud = Convert.ToString(Int64.Parse(str_data[i + 1]));
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        }
                        else
                        {
                            break;
                        }

                    default:
                        break;
                }
            }
            _ = thinAsync();
        }

        private async Task thinAsync()
        {
            var client = new ThingSpeakClient(false);
            var dataFeed = new ThingSpeakFeed
            {
                Field2 = Temperatura,
                Field3 = Temperatura1,
                Field4 = Temperatura2,
                Field5 = LDR,
                Field6 = str_PAtmosferica,
                Field7 = Altitud,
                Field8 = Humedad,
            };
            dataFeed = await client.UpdateFeedAsync("QF2EKNT0LC13VJJE", dataFeed);
        }

        public string ReadString
        {
            get { return str_ReadString; }
            set { Set(() => ReadString, ref str_ReadString, value); }
        }
        public string TimerReadString
        {
            get { return str_TimeReadString; }
            set { Set(() => TimerReadString, ref str_TimeReadString, value); }
        }

        public string Humedad
        {
            get { return str_Humedad; }
            set { Set(() => Humedad, ref str_Humedad, value); }
        }
        public string Temperatura
        {
            get { return str_Temperatura; }
            set { Set(() => Temperatura, ref str_Temperatura, value); }
        }
        public string Temperatura1
        {
            get { return str_Temperatura1; }
            set { Set(() => Temperatura1, ref str_Temperatura1, value); }
        }
        public string Temperatura2
        {
            get { return str_Temperatura2; }
            set { Set(() => Temperatura2, ref str_Temperatura2, value); }
        }
        public string LDR
        {
            get { return str_Ldr; }
            set { Set(() => LDR, ref str_Ldr, value); }
        }
        public string PAtmos
        {
            get { return str_PAtmosferica; }
            set { Set(() => PAtmos, ref str_PAtmosferica, value); }
        }
        public string Altitud
        {
            get { return str_Altitud; }
            set { Set(() => Altitud, ref str_Altitud, value); }
        }
        public string Intervalo
        {
            get { return str_Intervalo; }
            set { Set(() => Intervalo, ref str_Intervalo, value); }
        }


        public bool ChekedLed1
        {
            get { return bool_ChekedLed1; }
            set { Set(() => ChekedLed1, ref bool_ChekedLed1, value); }
        }
        private async void Led1Async()
        {
            if (ChekedLed1 == true)
            {
                await SentDataPortAsync("B,1");
            }
            else
            {
                await SentDataPortAsync("B,0");
            }

        }

        public bool ChekedLed2
        {
            get { return bool_ChekedLed2; }
            set { Set(() => ChekedLed2, ref bool_ChekedLed2, value); }
        }
        private async void Led2Async()
        {
            if (ChekedLed2 == true)
            {
                await SentDataPortAsync("C,1");
            }
            else
            {
                await SentDataPortAsync("C,0");
            }
        }

        public bool ChekedLedAll
        {
            get { return bool_ChekedLedAll; }
            set { Set(() => ChekedLedAll, ref bool_ChekedLedAll, value); }
        }
        private async void LedAllAsync()
        {
            if (ChekedLedAll == true)
            {
                await SentDataPortAsync("A,1");
                ChekedLed1 = true;
                ChekedLed2 = true;
            }
            else
            {
                await SentDataPortAsync("A,0");
                ChekedLed1 = false;
                ChekedLed2 = false;
            }
        }
        private async void ActualizarAsync()
        {
            await SentDataPortAsync("I," + str_Intervalo);
        }
        private async Task SentDataPortAsync(string value)
        {
            string string_data;
            string_data = char_StartMarker + value + char_EndMarker;
            await portService.sendToPort(string_data);
        }
    }
}
