using System;
using System.Threading;
using System.Windows.Forms;

namespace DepthGuess
{
    class PrograssWindow : IDisposable
    {
        private Thread thread;
        private PrograssForm form;

        public PrograssWindow(string text, int max)
        {
            form = new PrograssForm(text, max);

            thread = new Thread(new ParameterizedThreadStart((object data) =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run((Form)data);
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(form);

        }

        public void Add()
        {
            if (!form.IsDisposed)
                form.BeginInvoke(new Action(() =>{form.Add();}));
        }

        public void Close()
        {
            if (!form.IsDisposed)
                form.BeginInvoke(new Action(() => { form.Close(); }));
        }

        public void Join()
        {
            thread.Join();
        }

        private bool disposed = false;
        ~PrograssWindow()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool isDisposing)
        {
            if (!disposed)
            {
                if (isDisposing)
                {
                    if (thread.IsAlive)
                    {
                        form.BeginInvoke(new Action(() => { form.Close(); }));
                        thread.Join();
                    }
                }
                disposed = true;
            }
        }

    }
}
