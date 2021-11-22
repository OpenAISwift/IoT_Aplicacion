using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace IoT_Aplicacion_UWP.Utils
{
    public class SerialPortService
    {
        private CancellationTokenSource ReadCancellationTokenSource = new CancellationTokenSource();

        private SerialDevice class_serialDevice;
        private DataWriter class_dataWriter;
        private DataReader class_dataReader;

        public event EventHandler Event_Data;

        public SerialPortService()
        {
            GetPort();
        }

        private async void GetPort()
        {
            string qFilter = SerialDevice.GetDeviceSelector("COM23");
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(qFilter);
            if (devices.Any())
            {
                string deviceId = devices.First().Id;
                await OpenPort(deviceId);
            }
        }

        private async Task OpenPort(string deviceId)
        {
            class_serialDevice = await SerialDevice.FromIdAsync(deviceId);

            if (class_serialDevice != null)
            {
                class_serialDevice.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                class_serialDevice.ReadTimeout = TimeSpan.FromMilliseconds(100);
                class_serialDevice.BaudRate = 115200;
                class_serialDevice.Parity = SerialParity.None;
                class_serialDevice.StopBits = SerialStopBitCount.One;
                class_serialDevice.DataBits = 8;
                class_serialDevice.IsDataTerminalReadyEnabled = true;
                class_serialDevice.Handshake = SerialHandshake.None;
                await Listen();
            }
        }
        private async Task Listen()
        {
            try
            {
                if (class_serialDevice != null)
                {
                    class_dataReader = new DataReader(class_serialDevice.InputStream);
                    while (true)
                    {
                        await ReadAsync(ReadCancellationTokenSource.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Uart Error", ex);
            }
        }
        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;
            uint ReadBufferLength = 256;  // only when this buffer would be full next code would be executed
            // Si se solicitó la cancelación de la tarea, cumplir
            class_dataReader.InputStreamOptions = InputStreamOptions.Partial;
            // Configurar InputStreamOptions para completar la operación de lectura asíncrona cuando uno o más bytes están disponibles
            class_dataReader.InputStreamOptions = InputStreamOptions.Partial;
            // Crear un objeto de tarea para esperar los datos en el serialPort.InputStream
            loadAsyncTask = class_dataReader.LoadAsync(ReadBufferLength).AsTask(cancellationToken);
            // Lanzar la tarea y esperar hasta que el buffer esté lleno
            uint bytesRead = await loadAsyncTask;
            if (bytesRead > 0)
            {
                string strFromPort = class_dataReader.ReadString(bytesRead);
                Event_Data(strFromPort, null);
            }
        }
        private async Task WriteAsync(string text2write)
        {
            Task<UInt32> storeAsyncTask;

            if (text2write.Length != 0)
            {
                class_dataWriter.WriteString(text2write);

                storeAsyncTask = class_dataWriter.StoreAsync().AsTask();  // Create a task object

                UInt32 bytesWritten = await storeAsyncTask;   // Launch the task and wait
                if (bytesWritten > 0)
                {
                    //txtStatus.Text = bytesWritten + " bytes written at " + DateTime.Now.ToString(System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.LongTimePattern);
                }
            }
            else { }
        }
        public async Task sendToPort(string sometext)
        {
            try
            {
                if (class_serialDevice != null)
                {
                    class_dataWriter = new DataWriter(class_serialDevice.OutputStream);

                    await WriteAsync(sometext);
                }
                else { }
            }
            catch (Exception ex)
            {
                //txtStatus.Text = ex.Message;
            }
            finally
            {
                if (class_dataWriter != null)   // Cleanup once complete
                {
                    class_dataWriter.DetachStream();
                    class_dataWriter = null;
                }
            }
        }
        private void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
        }
        private async void DisplayNoPortDialog(string Val_StrEx)
        {
            ContentDialog noWifiDialog = new ContentDialog
            {
                Title = "Error Puerto Serie",
                Content = Val_StrEx,
                CloseButtonText = "Ok"
            };
            await noWifiDialog.ShowAsync();
        }
    }
}
