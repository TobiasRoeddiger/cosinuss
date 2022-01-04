# cosinuss° One Library
![Build Passing](https://img.shields.io/badge/build-passing-success)
![Open Issues](https://img.shields.io/github/issues/TobiasRoeddiger/cosinuss)
![Twitter Tobias Röddiger](https://img.shields.io/badge/twitter-%40TobiasRoeddiger-informational)

 
Third-party .NET library to read raw sensor data from the cosinuss° One Bluetooth earables (work in progress). NuGet publication pending.

| Feature  | Supported | BLE GATT API | Reference |
| ------------- | :-------------: | :-------------: |:-------------: |
| Device Information | ✅  | ✅  | [1] |
| Heart Rate  |  ✅ | ✅  | [1] |
| Body Temperature  | ✅  | ✅  | [1] |
| Sensor Quality | ✅  | ❌  | [2] |
| Step Frequency | ✅  | ❌  | [2] |
| Accelerometer X, Y, Z | 🚧  | ❌ | [2] |
| PPG Green | 🚧  | ❌ | [2] |
| PPG Red | 🚧  | ❌ | [2] |
| Ambient Light | 🚧  | ❌ | [2] |

[1] Bluetooth SIG, Generic Attribute Profile (GATT), January 2020, [URL](https://www.bluetooth.com/specifications/gatt/)

[2] Reverse-engineered funcationalities. **Use with caution and at your own risk! Correct functionality not guranteed.**

[3] Röddiger, Tobias, et al. "Towards Respiration Rate Monitoring Using an In-Ear Headphone Inertial
Measurement Unit" International Joint Conference on Pervasive and Ubiquitous Computing (Ubicomp'19), 1st
International Workshop on Earable Computing Systems. ACM, 2019.


## Sample App
<img src="https://github.com/TobiasRoeddiger/cosinuss/blob/master/art/info-graphic.png?raw=true" width="600">


## Supported Platforms
* .NET Standard
* Android 4.3+
* iOS 7+
* tvOS
* macOS
* UWP

## Setup

**Android**

Add the following to your AndroidManifest.xml
_PLEASE NOTE THAT YOU HAVE TO REQUEST THESE PERMISSIONS USING [Activity.RequestPermission](https://developer.android.com/training/permissions/requesting)_ or a [Plugin](https://github.com/jamesmontemagno/PermissionsPlugin). **The example app includes an implementation for the Permissions Plugin.**

```xml
<uses-permission android:name="android.permission.BLUETOOTH"/>
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN"/>

<!--this is necessary for Android v6+ to get the device name and address-->
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />

```

**iOS**

To use the cosinuss device as a background BLE periperhal, add the following to your Info.plist

```xml

<key>UIBackgroundModes</key>
<array>
    <!--for connecting to devices (client)-->
	<string>bluetooth-central</string>

    <!--for server configurations if needed-->
	<string>bluetooth-peripheral</string>
</array>

<!--To add a description to the Bluetooth request message (on iOS 10 this is required!)-->
<key>NSBluetoothPeripheralUsageDescription</key>
<string>YOUR CUSTOM MESSAGE</string>
```

## Usage
```c#
// TODO: ensure permissions are granted as shown in the previous section of the README

Cosinuss.Library.Cross.Current.OnDeviceFound += Current_OnDeviceFound;
Cosinuss.Library.Cross.Current.StartScanning();

void Current_OnDeviceFound(object sender, ICosinussDevice cosinussDevice)
{
    // stop scanning before connecting to avoid any issues
    Cosinuss.Library.Cross.Current.StopScanning();
    Cosinuss.Library.Cross.Current.OnDeviceFound -= Current_OnDeviceFound; // deregister event handlers

    // connect to the cosinuss device
    cosinussDevice.OnConnectionStateChanged += CosinussDevice_OnConnectionStateChanged;
    cosinussDevice.Connect();    
}

private void CosinussDevice_OnConnectionStateChanged(object sender, ConnectionState e)
{
    var cosinussDevice = (ICosinussDevice)sender;

    if (e == ConnectionState.CONNECTED)
    {
	cosinussDevice.BatteryLevelChanged += CosinussDevice_BatteryLevelChanged;
    	cosinussDevice.SensorQualityIndexChanged += CosinussDevice_DataQualityIndexChanged;

    	cosinussDevice.BodyTemperatureChanged += CosinussDevice_BodyTemperatureChanged;
    	cosinussDevice.HeartRateChanged += CosinussDevice_HeartRateChanged;
    	cosinussDevice.SPO2Changed += CosinussDevice_SPO2Changed;
	cosinussDevice.AccelerometerChanged += CosinussDevice_AccelerometerChanged;
    }
}
```
