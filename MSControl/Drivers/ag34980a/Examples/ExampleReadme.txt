			  DAC Example VI

Equipment Required:  This example expects a 34951 DAC card in slot 1 and a multiplexer
 card ( 34921 - 34925) in slot 2.

The example defines a trace named "Example Trace" on the 34951 DAC card.  It then outputs
that trace.  The output of the DAC card is assumed to be wired into channel 2505 of a
mulitplexer card in slot 2.  The closing of channels 2005 and 2921 routes the signal to
the 34980A's DMM.

The DMM then measures the DAC signal for AC Voltage and Frequency and the results are displayed.


			Mux Example VI

Equipment required:  a multiplexer card ( 34921 - 34925 ) in slot 2, and internal DMM present
 and enabled in the 34980.

The example shows how to configure multiple channels for different functions and different
 measurement ranges on those functions (besides auto-range).  You need to use Configure + Init
 + Fetch as shown here when you wish to:

  -- measure different functions in a single scan
  -- specify  triggering other than the default ( immediate)
  -- want to change the NPLC or Aperture time for faster reading rates.

 In contrast, the 'Measure' VIs found in the Data section force a trigger source of immediate
  and the same function measurement on the scanned channels.

			
			DigIO Example VI

Equipment required:  34952 multi-function card.

NOTE:  This example only configures the channels.  To see any real results, the user must:
   a) wire channel 006 to a DMM, either directly or through a multiplexer channel.
   b) wire an input signal into channel 5 for the totalizer to count edges.
   c) wire channel 1 output to channel 3 input to read 85 decimal for the digital input results. 

In the example, the following is configured:

   Channel 6 of the card is configured to output a voltage of 1.234 V.  

   Channel 5 is configured to Totalizer function, so it will count any signal input onto channel 5
 and display the result.  It is set to NOT clear the count when it is read.

   The digital IO port on channel 1 is configured to output a single byte of value 85 decimal.

   The digital IO port on channel 3 is configured to read a digital byte and the result is displayed.


                     CheckOper_45 VI

This vi looks at the description of the first 34945EXT module, presets all

banks on the module to default values for that distribution board type,

enables verification of the switch closure (using negative polarity), closes

a channel, reads the IsClosed state of that channel, and reads the drive

status of the channel.  (NOTE that the drive state will be "0" if the channel

is set for Pulsed operation).  NOTE also that inverted verification polarity

is needed with the N1810UL Switch.

This example assumes the following hardware configuration:
           - 34945A RF/Microwave Switch Driver
           - 34945EXT connected to 34945A with cable
           - (1) Y1150A Distribution Module in slot 1 of 34945EXT
           - (1) N1810UL Switch in SW1 socket

