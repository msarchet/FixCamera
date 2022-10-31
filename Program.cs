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

            List<DsDevice> devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice).ToList();

            // Filter that list down to the one with hyper-aggressive focus
            bool selected = false;

            DsDevice device = null;
            int index = 0;
            Console.WriteLine("Select Device");
            while (!selected)
            {
                foreach (var camera in devices)
                {
                    Console.WriteLine($"{index} {camera.Name}");
                }

                var selectedIndex = Console.ReadLine();

                if (int.TryParse(selectedIndex, out int selection))
                {
                    if (selection >= 0 && selection < devices.Count())
                    {
                        device = devices[selection];
                        selected = true;
                        break;
                    }
                }

                Console.WriteLine("Invalid selection, try again.");
            }

                        // Get the list of connected video cameras
            if (new FilterGraph() is IFilterGraph2 graphBuilder)
            {
                // Create a video capture filter for the device
                graphBuilder.AddSourceFilterForMoniker(device.Mon, null, device.Name, out IBaseFilter capFilter);

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
