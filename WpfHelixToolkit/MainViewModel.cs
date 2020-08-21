using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Microsoft.Win32;
using System.ComponentModel;

namespace WpfHelixToolkit
{
  
    public class MainViewModel 
    {
         public MainViewModel()
         {
             FileOpen();
         }

         string OpenFileFilter = "3D model files (*.3ds;*.obj;*.lwo;*.stl)|*.3ds;*.obj;*.objz;*.lwo;*.stl";
        public  void FileOpen()
        {
            string CurrentModelPath = OpenFileDialog("models", null, OpenFileFilter, ".3ds");              
            var mi = new ModelImporter();
          this.Model =    mi.Load(CurrentModelPath, Dispatcher.CurrentDispatcher);
      
        }
        private string OpenFileDialog(string initialDirectory, string defaultPath, string filter, string defaultExtension)
        {
            var d = new OpenFileDialog();
            d.InitialDirectory = initialDirectory;
            d.FileName = defaultPath;
            d.Filter = filter;
            d.DefaultExt = defaultExtension;
            if (!d.ShowDialog().Value)
            {
                return null;
            }

            return d.FileName;
        }

        public Model3D Model
        {
            get;
            set;
        }

      
    }
}
