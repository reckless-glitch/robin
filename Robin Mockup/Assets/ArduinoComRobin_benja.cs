

/* ArduinoConnector by Alan Zucconi
* http://www.alanzucconi.com/?p=2979
*/
using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;

public class ArduinoComRobin_benja : MonoBehaviour
{

    /* The serial port where the Arduino is connected. */
    [Tooltip("The serial port where the Arduino is connected")]
    public string port = "COM4";
    public string[] ports;
    /* The baudrate of the serial port. */
    [Tooltip("The baudrate of the serial port")]
    public int baudrate = 9600;
    [Tooltip("the time per frame will be bigger than this!!!")]
    public int ReadTimeout = 10;
    public float timeoutCallAfterMS = 10000f;
    public float scanningMaxTime = 5000;
    private SerialPort stream;
    [Header("info")]
    public bool scanning = true;
    public float scanningTime = 0;
    public int updateLoop = 0;
    public int aynchCalls = 0;
    public int asynchLoops = 0;
    public float asynchTime;
    public int callsAnswered = 0;
    public int callAnsweredAtLoop = 0;
    public int callsError = 0;
    public int callErrorAtLoop = 0;

    public delegate bool stringDelegate(string somestring);
    public stringDelegate onToken;

    public bool Open()
    {
        // Opens the serial port
        stream = new SerialPort(port, baudrate);
        stream.ReadTimeout = ReadTimeout;
        if (!stream.IsOpen)
        {
            try
            {
                Debug.Log("opening port " + stream.PortName);
                stream.Open();
            }
            catch(Exception)
            {
                Debug.Log("opening port failed");
                return false;
            }
            arduinoConnected = true;
        }
        if (!stream.IsOpen)
            return false;
        Debug.Log("port is open");
        return true;
        
        //this.stream.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
    }

    public void Close()
    {
        if(stream!= null && stream.IsOpen)
            stream.Close();
    }

    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        Debug.Log("start listening");
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);
        string dataString = null;
        reading = true;
        bool dataReceived = false;
        bool EOMreceived = false;
        asynchLoops = 0;
        float millis = 0;
        while (dataReceived || millis < timeout)
        {

            asynchLoops++;
            millis = diff.Milliseconds + diff.Seconds * 1000;
            // A single read attempt
            try
            {
                if (stream != null)
                    dataString = stream.ReadLine();
                else
                    dataString = null;
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            dataReceived = (dataString != null);

            if (dataReceived)
            {
                if (dataString == "EOM")
                {
                    Debug.Log("EOM received "+ aynchCalls + " / " + asynchLoops + " / " + asynchTime);
                    if (!EOMreceived)
                    {
                        //this is the first response
                        timeout = Mathf.Min(3 * millis, timeout); //reduce timeout
                    }
                    EOMreceived = true;

                }
                else
                {
                    callback(dataString);
                }
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
            }

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;
            asynchTime = millis;
        }
        reading = false;
        if (!EOMreceived && fail != null)
            fail();
        Debug.Log("stop reading " + aynchCalls + "/" + asynchLoops + "/" + asynchTime);
        //yield return null;
    }


    public void getArduinoState()
    {
        WriteToArduino("state");
        aynchCalls++;
        StartCoroutine
        (
            AsynchronousReadFromArduino
            ((string s) => onArduinoState(s),     // Callback
                () => onArduinoFail(), // Error callback
                timeoutCallAfterMS             // Timeout (milliseconds)
            )
        );
        Debug.Log("started ");
    }

    public void resetArduino()
    {
        Debug.Log("resetting arduino");
        if(WriteToArduino("reset"))
        {
            aynchCalls++;
            StartCoroutine
            (
                AsynchronousReadFromArduino
                ((string s) => onArduinoReset(s),     // Callback
                    () => onArduinoResetFail(), // Error callback
                    timeoutCallAfterMS             // Timeout (milliseconds)
                )
            );
        }
    }

    /// <summary>
    /// will start scanning simulation on arduino
    /// </summary>
    /// <param name="time">scanning time in seconds, 0 or empty will stop immediatly</param>
    public void scan(float time = 0)
    {
        if (time > 0)
        {
            WriteToArduino("start");
            scanningTime = time;
            scanning = true;
        }
        else
        {
            WriteToArduino("stop");
            scanningTime = time;
            scanning = false;
        }
    }

    void updateScanning()
    {
        if(scanning)
        {
            scanningTime -= Time.deltaTime;
            if(scanningTime<0)
            {
                scan(0);
            }
        }
    }

    public void onArduinoResetFail()
    {
        Debug.Log("error (" + aynchCalls + "/" + asynchLoops + "/" + asynchTime + ") :");
        callsError++;
    }

    public bool arduinoConnected = false;
    public int comPortID = 0;

    public void onArduinoReset(string answer = "")
    {
        if (answer == "reset robin")
        {
            if (!arduinoConnected)
            {
                arduinoConnected = true;
            }
        }
        else
            onArduinoResetFail();
    }

    public void onArduinoFail()
    {
        Debug.Log("error (" + aynchCalls + "/" + asynchLoops + "/" + asynchTime + ") :");
        callsError++;
    }



    public void onArduinoState(String state)
    {
        Debug.Log("message from arduino (" + aynchCalls + "/" + asynchLoops + "/" + asynchTime+") " + state);
        if (state.Length>5 && state.Substring(0,5)== "state")
        {
            // state:0/0/0
            // 01234567890
            callsAnswered++;
            setBlutooth(state.Substring(6, 1) == "1");
            setToken(state.Substring(10, 1));
            Debug.Log("state received bt:" + state.Substring(6, 1) + " tk:" + state.Substring(10, 1));
        }
        else if (state == "start" || state == "stop" || state == "reset robin")
        {
            Debug.Log(state + "received");
        }
        else
        {
            Debug.Log("strange message :/");
        }

    }

    public bool blutoothConnected = false;
    public string tokenState = "";
    public void setBlutooth(bool btState)
    {
        if (btState == blutoothConnected) return;

        if(btState && !blutoothConnected)
        {
            Debug.Log("blutooth connection established :) ");
        }
        else if(!btState && blutoothConnected)
        {
            Debug.Log("blutooth connection lost :( ");
        }

        blutoothConnected = btState;
    }

    public void setToken(string token)
    {
        if (token != tokenState)
            if(token=="0")
            {
                Debug.Log("no token");
            }
           else if (token == "?")
            {
                Debug.Log("unknown token");
            }
           else 
            {
                if (onToken != null)
                {
                    onToken(token);
                }
            }
        tokenState = token;
    }

    public bool WriteToArduino(string message)
    {
        // Send the request
        if (stream != null && stream.IsOpen)
        {
            Debug.Log("sending to arduino (port: "+ stream.PortName+"): " + message);
            try
            {
                stream.WriteLine(message);
                stream.BaseStream.Flush();
            }
            catch (TimeoutException)
            {
                return false;
            }
            Debug.Log("not sent - timeout");
            return true;
        }
        Debug.Log("send to arduino failed: " + message);
        return false;
    }


    public string ReadFromArduino(int timeout = 0)
    {
        stream.ReadTimeout = timeout;
        try
        {
            return stream.ReadLine();
        }
        catch (TimeoutException)
        {
            return null;
        }
    }



    public void Reset()
    {
        ports = SerialPort.GetPortNames();
        if (!arduinoConnected)
        {
            if (port == "") port = ArduinoConnector.AutodetectArduinoPort();
            Open();
        }
        resetArduino();
    }

    // Use this for initialization
    void Start()
    {
        ports = SerialPort.GetPortNames();
        //Open();
        //findArduino(0);
    }

    private void OnApplicationQuit()
    {
        Close();
    }

    public bool updateState = false;
    public bool reading = false;

    // Update is called once per frame
    void Update()
    {
        updateLoop++;
        if (arduinoConnected && updateState && !reading ) getArduinoState();
        updateScanning();
    }
}
