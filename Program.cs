using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixCamera
{
    class Program
    {
        static void Main(string[] args)
        {

            DsDevice[] devs = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
                        
            // Filter that list down to the one with hyper-aggressive focus
            var dev = devs.Where(d => d.Name.Equals("Microsoft® LifeCam Studio(TM)")).FirstOrDefault();

                        // Get the list of connected video cameras
            if (new FilterGraph() is IFilterGraph2 graphBuilder)
            {
                // Create a video capture filter for the device
                graphBuilder.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out IBaseFilter capFilter);

                // Cast that filter to IAMCameraControl from the DirectShowLib
                IAMCameraControl _camera = capFilter as IAMCameraControl;

                // Get the current focus settings from the webcam
                _camera.Get(CameraControlProperty.Focus, out int v, out CameraControlFlags f);

                // If the camera was not in manual focus mode, lock it into manual at the current focus setting
                if (f != CameraControlFlags.Manual)
                {
                    _camera.Set(CameraControlProperty.Focus, v, CameraControlFlags.Manual);
                }

                int pMin; 
                int pMax; 
                int pSteppingDelta; 
                int pDefault; 
                CameraControlFlags pCapsFlags;

                _camera.GetRange(CameraControlProperty.Focus, out pMin, out pMax, out pSteppingDelta, out pDefault, out pCapsFlags);

                Console.WriteLine($"{pMin} - {pMax} = {pSteppingDelta}");

                bool quit = false;
                while(!quit)
                {
                    string inline = Console.ReadLine(); 
                    if (int.TryParse(inline, out int value))
                    {
                        _camera.Set(CameraControlProperty.Focus, value, CameraControlFlags.Manual);
                    }
                    else if (inline == "quit")
                    {
                        quit = true;
                    }
                    
                }
            }
        }
    }
}
