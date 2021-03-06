// RegisterAssembly: DefinitionsCore.dll
// RegisterAssembly: BasicHardware.dll
// RegisterAssembly: PluginManager.dll
// RegisterAssembly: HardwareClient.dll
// RegisterSystemAssembly: System.dll
// RegisterAssembly: SpectrumLibrary.dll
// RegisterSystemAssembly: System.Xml.dll
// RegisterAssembly: Util.dll

/*
test file Ronny
*/

using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using Thermo.Imhotep.BasicHardware;
using Thermo.Imhotep.Definitions.Core;
using Thermo.Imhotep.SpectrumLibrary;
using Thermo.Imhotep.Util;



//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// main class of the RemoteControl script.
	// </summary>
//-------------------------------------------------------------------------------------------------------------------------------------------------------------
class RemoteControlTesting
{
	// declaration of class properties
	private static int m_PORT = 1069; // UDP Port for network connection
	
	private static Socket UDP_SOCK;
	private static Thread SERVER_THREAD;
	private static TcpListener TCPLISTENER;

	private static bool USE_UDP=true; // select what network protocol to use: UDP?
	private static bool TAG_DATA=true;

	private static bool USE_BEAM_BLANK=true;

	private static bool MAGNET_MOVING=false;
	private static double MAGNET_MOVE_THRESHOLD=0.25; // threshold to trigger stepped magnet move in DAC units
	private static int MAGNET_STEPS=20; // number of steps to divide the total magnet displacement
	public static int MAGNET_STEP_TIME=100; //ms to delay between magnet steps

	private static double LAST_Y_SYMMETRY=0;
	private static bool IsBLANKED=false;
	private static Dictionary<string, double> m_defl = new Dictionary<string, double>(); // dictionary that holds the deflection values

	public const int ON = 1;
	public const int OFF = 0;

	public const int OPEN = 1;
	public const int CLOSE = 0;
	
	public static IRMSBaseCore Instrument;
	public static IRMSBaseMeasurementInfo m_restoreMeasurementInfo;

	public static string SCAN_DATA;
	public static object m_lock=new object();

	
	// declaration of class methods
	public static void Main ()
	{
		Instrument= HelixMC;
		
		//init parameters
		Instrument.GetParameter("Y-Symmetry Set", out LAST_Y_SYMMETRY);
		
		//GetTuningSettings();
		PrepareEnvironment();
		
        //setup data recording
		InitializeDataRecord();
		
		// don't start UDPServer, this is only for testing
		/*
		if (USE_UDP)
		{
			UDPServeForever();
		}
		else
		{
		 	TCPServeForever();
		}
		*/
		
		// ##############################################################
		// ##############################################################
		// run tests here
		string cmd = 'GetData'
		Logger.Log (LogLevel.UserInfo, "execute command: {0}",cmd);
		string result = ParseAndExecuteCommand (cmd);
		Logger.Log (LogLevel.UserInfo, result);
		MessageBox.Show(string.Format("command: {0}; Result: {1}",cmd,result), "Resulting Data", "OK", "Information");
		// ##############################################################
	}
	
    //====================================================================================================================================
	// 
	//	Commands are case sensitive and in CamelCase
	//  do not include the "<" or ">" in the commands. 
	//  e.g SetTrapVoltage 120 not SetTrapVoltage <120>
	//	Commands:
	//		GetTuningSettingsList #return a comma separated string
	//		SetTuningSettings
	//		GetData returns tagged data e.g. H2,aaa,L1,bbb,CDD,ccc
	//		SetIntegrationTime <seconds>
	//      GetIntegrationTime <seconds>
	// 		BlankBeam <true or false> if true set y-symmetry to -50 else return to previous value
    
    //===========Cup/SubCup Configurations==============================
    // 		GetCupConfigurationList
	//		GetSubCupConfigurationList <cup name>
	//		GetActiveCupConfiguration
	//		GetActiveSubCupConfiguration
	// 		GetSubCupParameters returns list of Deflection voltages and the Ion Counter supply voltage
	//		SetSubCupConfiguration <sub cup name>
	//		SetCupConfiguration <cup name> <sub cup name>
	
	//===========Ion Counter============================================
	//      ActivateIonCounter
	//      DeactivateIonCounter
	
	//===========Ion Pump Valve=========================================
    //      Open  #open the Ion pump to the mass spec
	//		Close #closes the Ion pump to the mass spec
	//		GetValveState #returns True for open false for close
	
	//===========Magnet=================================================
	//		GetMagnetDAC
	//		SetMagnetDAC <value> #0-10V
	//      GetMagnetMoving
	//      SetMass <value>
	
	//===========Source=================================================
	//		GetHighVoltage or GetHV
	//		SetHighVoltage or SetHV <kV>
	//      GetTrapVoltage
	//      SetTrapVoltage <value>
    //      GetElectronEnergy
    //      SetElectronEnergy <value>
    //      GetYSymmetry
    //      SetYSymmetry <value>
    //      GetZSymmetry
    //      SetZSymmetry <value>
    //      GetZFocus
    //      SetZFocus <value>
    //      GetIonRepeller
    //      SetIonRepeller <value>
    //      GetExtractionLens
    //      SetExtractionLens <value>
    
    //==========Detectors===============================================
    //      ProtectDetector <name>,<On/Off>
	//      GetDeflection <name>
	//      SetDeflection <name>,<value>
	//      GetIonCounterVoltage
	//      SetIonCounterVoltage <value>
	//==================================================================
	//		Error Responses:
	//			Error: Invalid Command   - the command is poorly formated or does not exist. 
	//			Error: could not set <hardware> to <value> 
	
    //==========Generic Device===============================================
	//      GetParameter <name> -  name is any valid device currently listed in the hardware database
	//      SetParameter <name>,<value> -  name is any valid device currently listed in the hardware database
	//====================================================================================================================================
	
	
	
	
	
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// Parse the command, select the appropriate methode/function that executes the command.
	// command and list of arguments is separated by ' '
	// parameters that are typically separated by ','
	// </summary>
	// <param name="cmd">The command string: command parameter(s)</param>
	// <returns>None</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	private static string ParseAndExecuteCommand (string cmd)
	{
		
		string result = "Error: Invalid Command";
		Logger.Log(LogLevel.Debug, String.Format("Executing {0}", cmd));
		
		// command and list of arguments is separated by ' '
		// args[0] is the command and args[1] is the sring of parameters that are typically separated by ','
		string[] args = cmd.Trim().Split (' ');
		string[] pargs;
		string jargs;

		double r;
		// use the command stored in args[0] to select the appropriate function
		switch (args[0]) {

			case "GetTuningSettingsList":
				result = GetTuningSettings();
				break;
				
			case "SetTuningSettings":
				if(SetTuningSettings(args[1]))
				{
					result="OK";
				}
				else
				{
					result=String.Format("Error: could not set tuning settings {0}",args[1]);
				}
				break;
				
			case "GetData":
				result=SCAN_DATA;
				break;
				
			case "SetIntegrationTime":
				result=SetIntegrationTime(Convert.ToDouble(args[1]));
				break;
				
			case "GetIntegrationTime":
				result=GetIntegrationTime();
				break;
				
			case "BlankBeam":
			
				if (!USE_BEAM_BLANK)
				{	
					result="OK";
					break;
				}
				
				double yval=LAST_Y_SYMMETRY;
				bool blankbeam=false;
				if (args[1].ToLower()=="true")
				{
					if(!IsBLANKED)
					{
						//remember the non blanking Y-Symmetry value
						Instrument.GetParameter("Y-Symmetry Set", out LAST_Y_SYMMETRY);
						yval=-50;
						IsBLANKED=true;
						blankbeam=true;
					}
				}
				else
				{
					if(IsBLANKED)
					{
						IsBLANKED=false;
						blankbeam=true;
						result=SetParameter("Y-Symmetry Set",yval);
					}
				}
				
				result="OK";
				if(blankbeam)
				{
					result=SetParameter("Y-Symmetry Set",yval);
				}
			
				break;
			
				//============================================================================================
				//   Cup / SubCup Configurations
				//============================================================================================					
			case "GetCupConfigurationList":
				List<string> cup_names = GetCupConfigurations ();
				result = string.Join ("\r", cup_names.ToArray ());
				break;
			
			case "GetSubCupConfigurationList":
				string config_name = args[1];
				List<string> sub_names = GetSubCupConfigurations (config_name);
				result = string.Join ("\r", sub_names.ToArray ());
				break;
				
			case "GetActiveCupConfiguration":
				result=Instrument.CupConfigurationDataList.GetActiveCupConfiguration().Name;
				break;
				
			case "GetActiveSubCupConfiguration":
				result=Instrument.CupConfigurationDataList.GetActiveSubCupConfiguration().Name;
				break;
				
			case "GetSubCupParameters":
				result=GetSubCupParameters();
				break;
				
			case "SetSubCupConfiguration":
				// this only sets the subcupconfiguration of the Argon cup configuration
				Logger.Log(LogLevel.Debug, String.Format("Executing {0}",cmd));
				if(ActivateCupConfiguration ("Argon", cmd.Remove(0,23)))
				{
					result="OK";
				}
				else
				{
					result=String.Format("Error: could not set sub cup to {0}", args[1]);
				}
				break;
			
			case "SetCupConfiguration":
				// this sets the cup configuration and its subcup configuration
				Logger.Log(LogLevel.Debug, String.Format("Executing {0}",cmd));
				if(ActivateCupConfiguration (args[1], args[2]))
				{
					result="OK";
				}
				else
				{
					result=String.Format("Error: could not set cup to {0} and sub cup to {1}", args[1], args[2]);
				}
				break;		
			
			//============================================================================================
			//   Ion Counter
			//============================================================================================					
			case "ActivateIonCounter":
				result="OK";
				SetIonCounterState(true);
				break;
				
			case "DeactivateIonCounter":
				result="OK";
				SetIonCounterState(false);
				break;
			
			//============================================================================================
			//   Ion Pump Valve
			//============================================================================================					
			case "Open":
				Logger.Log(LogLevel.Debug, String.Format("Executing {0}", cmd));
				//hardcode name for now
				result=SetParameter("Valve Ion Pump Set",OPEN);
				break;
				
			case "Close":
				Logger.Log(LogLevel.Debug, String.Format("Executing {0}", cmd));
				result=SetParameter("Valve Ion Pump Set",CLOSE);
				break;
				
			case "GetValveState":
				Logger.Log(LogLevel.Debug, String.Format("Executing {0}", cmd));
				result=GetValveState("Valve Ion Pump Set");
				Logger.Log(LogLevel.Debug, String.Format("Valve state {0}", result));
				break;

			//============================================================================================
			//   Magnet
			//============================================================================================					
			case "GetMagnetDAC":
				if (Instrument.GetParameter("Field Set", out r))
				{
					result=r.ToString();
				}
				break;
				
			case "SetMagnetDAC":		
				result=SetMagnetDAC(Convert.ToDouble(args[1]));
				break;
				
			case "GetMagnetMoving":
				result=GetMagnetMoving();
				break;
				
			case "SetMass":
				result = "Ok";
				RunMonitorScan(Convert.ToDouble(args[1]));
				break;
				
			//============================================================================================
			//    Source Parameters
			//============================================================================================			
			case "GetHighVoltage":
				if(Instrument.GetParameter("Acceleration Reference Set",out r))
				{
					result=(r*1000).ToString();
				}
				break;
				
			case "SetHighVoltage":
				result=SetParameter("Acceleration Reference Set", Convert.ToDouble(args[1])/1000.0);
				break;
			case "GetHV":
				if(Instrument.GetParameter("Acceleration Reference Set",out r))
				{
					result=(r).ToString();
				}
				break;
				
			case "SetHV":
				result=SetParameter("Acceleration Reference Set", Convert.ToDouble(args[1])/1000.0);
				break;
			case "GetTrapVoltage":
				if(Instrument.GetParameter("Trap Voltage Readback",out r))
				{
					result=r.ToString();
				}
				break;
				
			case "SetTrapVoltage":
				result=SetParameter("Trap Voltage Set",Convert.ToDouble(args[1]));
				break;
			
			case "GetElectronEnergy":
				if(Instrument.GetParameter("Electron Energy Readback",out r))
				{
					result=r.ToString();
				}
				break;
				
			case "SetElectronEnergy":
				result=SetParameter("Electron Energy Set",Convert.ToDouble(args[1]));
				break;
				
			case "GetIonRepeller":
				if(Instrument.GetParameter("Ion Repeller Set",out r))
				{
					result=r.ToString();
				}
				break;
				
			case "SetIonRepeller":
				result=SetParameter("Ion Repeller Set",Convert.ToDouble(args[1]));
				break;
			
			case "GetYSymmetry":
				if(Instrument.GetParameter("Y-Symmetry Set",out r))
				{
					result=r.ToString();
				}
				break;
				
			case "SetYSymmetry":
				LAST_Y_SYMMETRY=Convert.ToDouble(args[1]);
				result=SetParameter("Y-Symmetry Set",Convert.ToDouble(args[1]));
				break;
			
			case "GetZSymmetry":
				if(Instrument.GetParameter("Z-Symmetry Set",out r))
				{
					result=r.ToString();
				}
				break;
				
			case "SetZSymmetry":
				result=SetParameter("Z-Symmetry Set",Convert.ToDouble(args[1]));
				break;
				
			case "GetZFocus":
				if(Instrument.GetParameter("Z-Focus Set",out r))
				{
					result=r.ToString();
				}
				break;
			
			case "SetZFocus":
				result=SetParameter("Z-Focus Set",Convert.ToDouble(args[1]));
				break;
			
			case "GetExtractionLens":
				if(Instrument.GetParameter("Extraction Lens Set",out r))
				{
					result=r.ToString();
				}
				break;
				
			case "SetExtractionLens":
				result=SetParameter("Extraction Lens Set",Convert.ToDouble(args[1]));
				break;
		
			//============================================================================================
			//    Detectors
			//============================================================================================			
			case "ProtectDetector":
				pargs=args[1].Split(',');
				ProtectDetector(pargs[0], pargs[1]);
				break;

			case "GetDeflection":
				//jargs=String.Join(" ", args.Slice(1,-1));
				if(Instrument.GetParameter(String.Format("Deflection {0} Set",args[0]), out r))
				{
					result=r.ToString();
					//result="OK";
				}
				break;
				
			case "SetDeflection":
				//jargs=String.Join(" ", args.Slice(1,-1));
				//pargs=jargs.Split(',');
				pargs=args[1].Split(',');
				result=SetParameter(String.Format("Deflection {0} Set",pargs[0]),Convert.ToDouble(pargs[1]));
				break;
				
			case "GetIonCounterVoltage":
				//if(Instrument.GetParameter("CDD Supply Set",out r))
				if(Instrument.GetParameter("CDD Supply Set",out r))
				{
					result=r.ToString();
				}
				break;
				
			case "SetIonCounterVoltage":
				pargs=args[1].Split(',');
				//result=SetParameter("CDD Supply Set", Convert.ToDouble(args[1]));
				result=SetParameter(String.Format("{0} Supply Set",pargs[0]),Convert.ToDouble(pargs[1]));
				break;
		    	    
		//============================================================================================
		//    Generic
		//============================================================================================			
			case "GetParameter":
				if(Instrument.GetParameter(args[1], out r))
				{
					result=r.ToString();
				}
				break;
				
			case "SetParameter":
				pargs=args[1].Split(',');
				result=SetParameter(pargs[0], Convert.ToDouble(pargs[1]));
				break;
			}

		return result;
	}
	
	
//============================================================================================
//    EOCommands
//============================================================================================          

	private static bool PrepareEnvironment()
    {
        m_restoreMeasurementInfo=Instrument.MeasurementInfo;
        return Instrument.ScanTransitionController.InitializeScriptScan(m_restoreMeasurementInfo);
    }
      
	public static void InitializeDataRecord()
	{
		// attach a handler to the ScanDataAvailable Event
		Instrument.ScanDataAvailable+=ScanDataAvailable;
	}
	
	public static void Dispose()
	{
		Logger.Log (LogLevel.UserInfo, "Stop Server");
	
		// deattach the handler from the ScanDataAvailable Event
		Instrument.ScanDataAvailable-=ScanDataAvailable;
		
		//shutdown the server
		// don't use this is just for testing
		/*
		if (USE_UDP)
		{
			UDP_SOCK.Close();			
		}
		else
		{
			TCPLISTENER.Stop();
		}
		
		SERVER_THREAD.Abort();
		*/
	}
	
//====================================================================================================================================
//Qtegra Methods
//====================================================================================================================================
	public static string GetMagnetMoving()
	{
		if (MAGNET_MOVING)
		{
			return "True";
		}
		else
		{
			return "False";
		}
	}
	
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// protect or unprotect the detector by setting the deflection to 3250 or 0 (or the previous unprotected value)
	// </summary>
	// <param name="detname">Name of the detector</param>
	// <param name="state">state of the protection: on=protect, off=unprotect</param>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------

	public static void ProtectDetector(string detname, string state)
	{
	    Logger.Log (LogLevel.UserInfo, "performing: ProtectDetector");
		string param=String.Format("Deflection {0} Set",detname);
		Logger.Log (LogLevel.UserInfo, param);
		
	    if (state.ToLower()=="on")
		// protect detector
		{
	    	double v;
			// get current deflection value
	        Instrument.GetParameter(param, out v);
			if (v<3250)
			// only protect if deflection is <3250 (unprotected)
			{
				m_defl[detname]=v;
				// set deflection to 3250 (maximum)
				SetParameter(param, 3250);
			}
	    }
	    else
		// unprotect detector by setting it to the value before protection
	    {
	        if (m_defl.ContainsKey(detname))
	        {
	            SetParameter(param, m_defl[detname]);
	        }
			else
			// no value before protection was given, set deflection to 0
			{
				SetParameter(param, 0);
			}
	    }
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// DOESN'T work yet.
	// set the state of the ion counter
	// </summary>
	// <param name="state">state of the protection: true=protect, false=unprotect</param>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	public static void SetIonCounterState(bool state)
	{
		if (state)
		{
			Logger.Log (LogLevel.UserInfo, "Setting IonCounterState True");
		}
		else
		{
			Logger.Log (LogLevel.UserInfo, "Setting IonCounterState False");
		}
		IRMSBaseCupConfigurationData activeCupData = Instrument.CupConfigurationDataList.GetActiveCupConfiguration();
		foreach(IRMSBaseCollectorItem col in activeCupData.CollectorItemList)
		{
		//Logger.Log (LogLevel.UserInfo, col.CollectorType)
			if ((col.CollectorType == IRMSBaseCollectorType.CounterCup) && (col.Mass.HasValue == true))
			{
			    col.Active = state;
			}
		}

	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// returns the state of the pump valve
	// </summary>
	// <param name="hwname">Name of the valve (Pump Valve = 'Valve Ion Pump Set'</param>
	// <returns><c>True</c> if valve opnen; <c>False</c> if valve closed.</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------

	public static string GetValveState(string hwname)
	{
		string result="Error";
		double rawValue;
		if(Instrument.GetParameter(hwname,out rawValue))
		{
			if (rawValue==OPEN)
			{
				result="True";
			}
			else
			{
				result="False";
			}
		}
		return result;
		
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// set a parameter in qtegra
	// </summary>
	// <param name="hwname">paremeter name as give in qtegra</param>
	// <param name="val">value of the parameter</param>
	// <returns><c>OK</c> if parameter was set successfully being; <c>Error: could not set parameter to value </c> if an error occured.</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	public static string SetParameter(string hwname, int val)
	{
		string result="OK";
		if (!Instrument.SetParameter(hwname, val))
		{
			result=String.Format("Error: could not set {0} to {1}", hwname, val);
		}
		return result;
	}
	
	public static string SetParameter(string hwname, double val)
	{	string result="OK";
	
	
		if (!Instrument.SetParameter(hwname, val))
		{
			result=String.Format("Error: could not set {0} to {1}", hwname, val);
		}
		return result;
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// set the DAC value of the magnet and move the magent there
	// </summary>
	// <param name="d">magnet DAC value</param>
	// <returns><c>OK</c> if parameter was set successfully</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------

	public static string SetMagnetDAC(double d)
	{
		
		string result="OK";
		double current_dac;
		
		if (Instrument.GetParameter("Field Set", out current_dac))
		{
			double dev=Math.Abs(d-current_dac);			
			if (dev>MAGNET_MOVE_THRESHOLD)
			{   
                Thread t= new Thread(delegate(){mSetMagnetDAC(d,dev,current_dac);});
                t.Start();
                result="OK";
			}
			else
			{
				result=SetParameter("Field Set", d);
			}
		}
		return result;
	}
	
	public static void mSetMagnetDAC(double d, double dev, double current_dac)
	{
		MAGNET_MOVING=true;
	    //incrementally move the magnet to eliminate "ringing"
	    double step=dev/MAGNET_STEPS;
	    int sign=1;

        if (current_dac>d)
        {
            sign=-1;
        }

        for(int i=1; i<=MAGNET_STEPS; i++)
        {
            SetParameter("Field Set", current_dac+sign*i*step);
            if (MAGNET_STEP_TIME>0)
            {
                Thread.CurrentThread.Join(MAGNET_STEP_TIME);
            }
        }
        MAGNET_MOVING=false;
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// get the integration time
	// </summary>
	// <returns>integration time in msec</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	public static string GetIntegrationTime()
	{
		string result=String.Format("{0}", Instrument.MeasurementInfo.IntegrationTime*0.001);
		return result;
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// set the integration time
	// </summary>
	// <param name="t">integration time in sec</param>
	// <returns><c>OK</c> if parameter was set successfully being; <c>Error: could not set integration time to t </c> if an error occured.</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	public static string SetIntegrationTime(double t)
	{
		//t in ms
		string result="OK";
		IRMSBaseMeasurementInfo nMI= new IRMSBaseMeasurementInfo(Instrument.MeasurementInfo);
		nMI.IntegrationTime = t*1000;
		if (!Instrument.ScanTransitionController.StartMonitoring (nMI))
		{
			Logger.Log(LogLevel.UserError, "Could not start the modified monitor");
			result=String.Format("Error: could not set integration time to {0}",t);
		}
		
		return result;
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// Scan Data Available
	// </summary>
	// <param name="sender">???</param>
	// <param name="e">???</param>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	public static void ScanDataAvailable(object sender, EventArgs<Spectrum> e)
	{ 
		lock(m_lock)
		{
			List<string> data = new List<string>();
			Spectrum spec = e.Value.Clone() as Spectrum;
			IRMSBaseCupConfigurationData cupData = Instrument.CupConfigurationDataList.GetActiveCupConfiguration();
	
			// change detnames to a list of detectors on your system
			// this is is for an Argus VI c. 2010
			//List<string> detnames = new List<string>(new string[]{"CUP 4,H2","CUP 3,H1",
			//													  "CUP 2,AX",
			//													  "CUP 1,L1","CUP 0,L2",
			//													  "CDD 0,CDD"
			//													  });

																  
			// list of cup names for LDEO HelixMCPlus
			List<string> detnames = new List<string>(new string[]{"CUP 4,H2",
			                                                      "CUP 3,H1",
																  "CUP 2,AX",
																  "CUP 1,L1",
																  "CUP 0,L2",
																  "CDD 0,L2_CDD",
																  "CDD 1,L1_CDD",
																  "CDD 2, AX_CDD",
																  "CDD 3, H1_CDD",
																  "CDD 4, H2_CDD"
																  });										  
			double cddMass=0;
			double cddCounts=0;
			bool cdd=false;
			foreach (Series series in spec)
			{
				foreach (SpectrumData point in series)
				{
				
					//get the name of the detector
					foreach (IRMSBaseCollectorItem item in cupData.CollectorItemList)
					{
						if (item.Mass==point.Mass)
						{	string cupName="";
							foreach (string detname in detnames)
							{
								string[] args=detname.Split(',');
								if(args[0]==item.Identifier)
								{
									cupName=args[1];
									break;
								}
							}
							//delegate adding the CDD value until later
							//this way its easy to put at the end of the data string
							//
							// cddMass and cddCounts should be changed to lists 
							// to handle mulitple CDD's for one machine
							//
							if( cupName=="CDD")
							{
								cdd=true;
								cddMass=point.Mass;
								cddCounts=point.Analog;
							}
							else
							{								
								data.Add(point.Analog.ToString());
								if (TAG_DATA)
								{
									data.Add(cupName);
								}
								
							}
							break;
						}
					}

				}
			}
			data.Reverse();
			if(cdd)
			{
				if (TAG_DATA)
				{
					data.Add("CDD");
				}
				data.Add(cddCounts.ToString());
			}
			
			SCAN_DATA=string.Join(",",data.ToArray());
		}
	}
	
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// run a monitor scan
	// </summary>
	// <param name="mass">mass value to set the magnet to</param>
	// <returns><c>True</c> if scan was performed</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	private static bool RunMonitorScan (double? mass)
	{
		IRMSBaseCupConfigurationData cupData = Instrument.CupConfigurationDataList.GetActiveCupConfiguration();
		IRMSBaseMeasurementInfo newMeasurementInfo = 
			new IRMSBaseMeasurementInfo(
				Instrument.MeasurementInfo.ScanType,
				Instrument.MeasurementInfo.IntegrationTime,
				Instrument.MeasurementInfo.SettlingTime,
				(mass.HasValue) ? mass.Value : cupData.CollectorItemList.GetMasterCollectorItem().Mass.Value,
				cupData.CollectorItemList,
				cupData.MassCalibration
				);
		Instrument.ScanTransitionController.StartMonitoring(newMeasurementInfo);
		return true;
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// run the currently active tuning settings
	// </summary>
	// <returns><c>tuning settings</c> string with the tuning settings separated by ','</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------

	private static string GetTuningSettings()
	{
		TuneSettingsManager tsm = new TuneSettingsManager(Instrument.Id);
		List<string> result = new List<string>();
		tsm.EntryType= typeof(TuneSettings);
		result.AddRange(tsm.GetEntries());
		return string.Join(",",result.ToArray());	
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// load the tuning 
	// </summary>
	// <param name="name">name of the tune file</param>
	// <returns><c>True</c> if loaded; <c>False<c> if not loaded</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	private static bool SetTuningSettings(string name)
	{
		
		TuneSettingsManager tsm = new TuneSettingsManager(Instrument.Id);
		TuneSettings tuneSettings = tuneSettings = tsm.ReadEntry(name) as TuneSettings; ;
		//TuneSettings tuneSettings =tsm.ReadEntry(name);
		TuneParameterBlock tuneBlock = null;
		if (tuneSettings == null)
		{
			Logger.Log(LogLevel.UserError, String.Format("Could not load tune setting \'{0}\'.", name));
			return false;
		}
		else
		{
			tuneBlock = tuneSettings.Object as TuneParameterBlock;
			if (tuneBlock == null)
			{
				Logger.Log(LogLevel.UserError, string.Format("Tune setting \'{0}\' could not convert to tuneblock.", name));
				return false;
			}
			else
			{
				Instrument.TuneParameters.Parameters = tuneBlock;
				Logger.Log(LogLevel.UserInfo, string.Format("Tune setting : \'{0}\' successfully load!", name));
			}
		}
		return true;
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// get currently loaded subcupparameters 
	// </summary>
	// <returns><c>subcupparameter</c> string that holds the supcupparameters</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	private static string GetSubCupParameters()
	{	
	    // change parameters to values relevant for your system
	    // default is Argus VI c. 2010
		//List<string> parameters = new List<string>(new string[]{
		//										"Deflection H2 Set",
		//										"Deflection H1 Set",
		//										"Deflection AX Set",
		//										"Deflection L1 Set",
		//										"Deflection L2 Set",
		//										"Deflection CDD Set",
		//										"CDD Supply Set",
		//											});

													
	    // LDEO HelixMCPlus
		List<string> parameters = new List<string>(new string[]{
	  											"Deflection H2_CDD Set",
												"Deflection H1_CDD Set",
												"Deflection AX_CDD Set",
												"Deflection L1_CDD Set",
												"Deflection L2_CDD Set",
												"H2_CDD Supply Set",
												"H1_CDD Supply Set",
												"AX_CDD Supply Set",
												"L1_CDD Supply Set",
												"L2_CDD Supply Set"});
		double value=0.0;
		List<string> data = new List<string>();
		foreach (var item in parameters)
		{
			if(Instrument.GetParameter(item, out value))
			{
				data.Add(String.Format("{0},{1}", item, value));
			}
		}
		return String.Join(";", data.ToArray());
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// Get all cup configurations names from the system.
	// </summary>
	// <returns>A list of cup configuration names.</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	private static List<string> GetCupConfigurations ()
	{
		List<string> result = new List<string>();
		foreach (var item in Instrument.CupConfigurationDataList) {
			result.Add (item.Name);
		}
		return result;
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// Get all sub cup configurations names from a cup cupconfiguration.
	// </summary>
	// <param name="cupConfigurationName"></param>
	// <returns>A list of sub cup configuration names.</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	private static List<string> GetSubCupConfigurations (string cupConfigurationName)
	{
		IRMSBaseCupConfigurationData cupData = Instrument.CupConfigurationDataList.FindCupConfigurationByName (cupConfigurationName);
		if (cupData == null) {
			Logger.Log (LogLevel.UserError, String.Format ("Could not find cup configuration \'{0}\'.", cupConfigurationName));
			return null;
		}
		
		List<string> result = new List<string>();
		foreach (var item in cupData.SubCupConfigurationList) {
			result.Add (item.Name);
		}
		return result;
	}
	
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	// <summary>
	// Activate a cup configuration and sub cup configuration.
	// </summary>
	// <param name="cupConfigurationName">The cup configuration name.</param>
	// <param name="subCupConfigurationName">The sub cup configuration name.</param>
	// <param name="mass">A nullable mass value. If has a value then use the mass for the monitor scan; otherwise use the master cup mass for the monitor scan.</param>
	// <returns><c>True</c> if successfull; otherwise <c>false</c>.</returns>
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	private static bool ActivateCupConfiguration (string cupConfigurationName, string subCupConfigurationName)
	{
		Logger.Log (LogLevel.UserInfo, "performing: ActivateCupConfiguration");
		//Console.WriteLine (cupConfigurationName);
		//Console.WriteLine (subCupConfigurationName);
		IRMSBaseCupConfigurationData cupData = Instrument.CupConfigurationDataList.FindCupConfigurationByName (cupConfigurationName);
		if (cupData == null) {
			Logger.Log (LogLevel.UserError, String.Format ("Could not find cup configuration \'{0}\'.", cupConfigurationName));
			return false;
		}
		IRMSBaseSubCupConfigurationData subCupData = cupData.SubCupConfigurationList.FindSubCupConfigurationByName (subCupConfigurationName);
		if (subCupData == null) {
			Logger.Log (LogLevel.UserError, String.Format ("Could not find sub cup configuration \'{0}\' in cup configuration.", subCupConfigurationName, cupConfigurationName));
			return false;
		}
		Instrument.CupConfigurationDataList.SetActiveItem (cupData.Identifier, subCupData.Identifier, Instrument.CupSettingDataList, null);
		Instrument.SetHardwareParameters (cupData, subCupData);
		bool success = Instrument.RequestCupConfigurationChange (Instrument.CupConfigurationDataList);
		if (!success) {
			Logger.Log (LogLevel.UserError, "Could not request a cup configuration change.");
			return false;
		}
		return true;
	}
	
//====================================================================================================================================
//Server Methods
//====================================================================================================================================

	private static void TCPServeForever ()
	{
		Logger.Log (LogLevel.UserInfo, "Starting TCP Server.");
		TCPLISTENER = new TcpListener (IPAddress.Any, m_PORT);
		TCPLISTENER.Start ();
		
		
		SERVER_THREAD = new Thread (new ThreadStart (TCPListen));
		SERVER_THREAD.Start ();
		
	}
	private static void UDPServeForever()
	{
		Logger.Log (LogLevel.UserInfo, "Starting UDP Server.");
		SERVER_THREAD = new Thread (new ThreadStart (UDPListen));
		SERVER_THREAD.Start ();
		
	}
	private static void UDPListen()
	{
		Logger.Log (LogLevel.UserInfo, "UDP Listening.");
		int recv;
		byte[] data= new byte[1024];
		
		IPEndPoint ipep = new IPEndPoint(IPAddress.Any, m_PORT);
		UDP_SOCK = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		UDP_SOCK.Bind(ipep);
		
		IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
		EndPoint remote = (EndPoint)(sender);
		
		while(true)
		{
			try
			{
				recv=UDP_SOCK.ReceiveFrom(data, ref remote);
				string rdata = Encoding.ASCII.GetString(data,0, recv);
				
				// this is executing the command received from the UDP connection
				// and returns the result which is send back to the network
				string result = ParseAndExecuteCommand(rdata.Trim()); 
				
				//Logger.Log(LogLevel.Debug, String.Format("Sending back {0}", result));
				UDP_SOCK.SendTo(Encoding.ASCII.GetBytes(result), remote);
			} catch (Exception e) {
			string error = string.Format ("Could not read from UDP sock. {0}", e.ToString ());
			Logger.Log(LogLevel.Warning, error);
			}
			
		}
	}
	private static void TCPListen ()
	{
		
		Logger.Log (LogLevel.UserInfo, "TCP Listening");
		
		while (true) {
			TCPHandle(TCPLISTENER.AcceptTcpClient ());
			
			//TcpClient client = TCPLISTENER.AcceptTcpClient ();
			//Thread clientThread = new Thread (new ParameterizedThreadStart (TCPHandle));
			//clientThread.Start (client);
		}
	}
	
	private static void TCPHandle (TcpClient tcpClient)
	{
		//TcpClient tcpClient = (TcpClient)client;
		
		NetworkStream _stream = tcpClient.GetStream ();
		//string response = Read (_stream);
		
		string result = ParseAndExecuteCommand (TCPRead(_stream).Trim());
		
		TCPWrite(_stream, result);
		
		tcpClient.Close();
	}
	
//====================================================================================================================================
//Network Methods
//====================================================================================================================================
	
	private static void TCPWrite (NetworkStream stream, string cmd)
	{
		if (stream.CanWrite) {
			Byte[] sendBytes = Encoding.UTF8.GetBytes (cmd);
			stream.Write (sendBytes, 0, sendBytes.Length);
		}
	}
	
	private static string TCPRead (NetworkStream stream)
	{
		int BufferSize = 1024;
		byte[] data = new byte[BufferSize];
		try {
			StringBuilder myCompleteMessage = new StringBuilder ();
			int numberOfBytesRead = 0;
			
			// Incoming message may be larger than the buffer size.
			do {
				numberOfBytesRead = stream.Read (data, 0, data.Length);
				
				myCompleteMessage.AppendFormat ("{0}", Encoding.ASCII.GetString (data, 0, numberOfBytesRead));
			} while (stream.DataAvailable);
			
			//string rd = string.Format ("Read Data: {0} the received text is '{1}'", numberOfBytesRead, myCompleteMessage);
			
			return myCompleteMessage.ToString ();
		} catch (Exception e) {
			string error = string.Format ("Could not read from the NetworkStream. {0}", e.ToString ());
			Logger.Log(LogLevel.Warning, error);
		}
		return string.Empty;
	}
}
