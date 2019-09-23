using UnityEngine;
using System.Collections;
using MonitorComponents;

public class UsingAPI : MonoBehaviour 
{
	Monitor monitor;
    MonitorInput highWaveMonitorInput;
   	MonitorInput lowWaveMonitorInput;
   
    void Awake()
    {
        monitor = new Monitor("My monitor");
        highWaveMonitorInput = new MonitorInput(monitor, "high wave", Color.red);
        lowWaveMonitorInput = new MonitorInput(monitor, "low wave", Color.magenta);

    }
   
    void Update ()
    {
        highWaveMonitorInput.Sample(Mathf.Sin(Mathf.PI * 2f * Time.time));
        lowWaveMonitorInput.Sample(Mathf.Sin(Mathf.PI * 2f * 1.1f * Time.time));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            monitor.Add(new MonitorEvent() {
                text = "Space",
                time = Time.time
            });
        }
    }
}