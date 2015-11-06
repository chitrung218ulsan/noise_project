using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NoiseMonitoring.Utils;
using System.Threading;
using System.Data.Entity;
using MySql.Data;
namespace NoiseMonitoring
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AsynSocketListener.DataPort = "8009";
            AsynSocketListener socketListener = AsynSocketListener.Instance;
            Thread thread = new Thread(new ThreadStart(socketListener.StartListenning));
            
            thread.Start();
            
            Context db = new Context();
        }
    }
}
