# cosinuss° One Library
Third-party .NET library to read raw sensor data from the cosinuss° Bluetooth earables.

## Supported Platforms
* .NET Standard
* Android 4.3+
* iOS 7+
* tvOS
* macOS
* UWP

## Setup

Be sure to install the cosinuss.NET nuget package in your project.

**Android**

Add the following to your AndroidManifest.xml
_PLEASE NOTE THAT YOU HAVE TO REQUEST THESE PERMISSIONS USING [Activity.RequestPermission](https://developer.android.com/training/permissions/requesting)_ or a [Plugin](https://github.com/jamesmontemagno/PermissionsPlugin). The example app includes an implementation for the Permissions Plugin.

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
