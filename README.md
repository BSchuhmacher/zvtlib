# Introduction

This library can be used to communicate with payment terminals according to the ZVT-Protocol specification.

The solution contains 4 seperate projects:
* CardTerminalLibrary: This library contains the actual ZVT-Protocol implementation. It is .netstandard21 compliant.
* CardTerminals.Tests: Command line utility that can be used to run/test several CardTerminalLibrary functions. It is .net6.0 compliant.
* ZvtEcrInterface: This library provides a very easy to use wrapper around the original CardTerminalLibrary. It is .netstandard21 compliant.
* ZvtEcrInterfaceTester: Provides a simple GUI for using/testing the ZvtEcrInterface and performing various payment terminal tasks.  It is .net6.0 compliant.



## ZvtEcrInterfaceTester 

Provides a simple GUI for using/testing the ZvtEcrInterface and performing various payment terminal tasks.

Usage:
1. Select desired port and baud rate
2. Click "Create Interface" (observe log window)
3. Click "Register" (observe log window)
4. Perform payment actions like "Pay", "Refund" etc.

The propertygrid will display payment results after a successful payment has been performed.

![ZvtEcrInterface Tester v1.4.0](https://user-images.githubusercontent.com/936992/73853437-22a41f00-4831-11ea-87ac-7bcc2b1c6bb3.png)

## ZvtEcrInterface

This library provides a very easy to use wrapper around the original CardTerminalLibrary.

1) Create communication interface and specify required serial port settings

The EnvironmentStatus event reports intermediate status information with respect to payments etc.

```c#
ZvtCommunication zvtCommunication = new ZvtCommunication(new ZvtSerialPortSettings("COM1", 115200), Console.Out);
zvtCommunication.EnvironmentStatus += delegate(IntermediateStatus status) {
  if (InvokeRequired) {
    Invoke(new Action(() => ZvtCommunicationOnEnvironmentStatus(status)));
    return;
  }

  Debug.WriteLine($"ZvtEnvironment Status: {status}");
};
```

2) Most terminals require an initial registration command before performing payment commands

```C#
try {
  var result = _ZvtCommunication?.Register();
  // evaluate result
} catch (Exception ex) {
  // handle exception
} 
```

3) Perform payments, refunds etc.

```C#
try {
  var result = _ZvtCommunication?.Pay(100);
  // evaluate result
} catch (Exception ex) {
  // handle exception
} 
```

That's it.

# Acknowledgments

Thanks to all the ones contributing to the source code and for sharing the original code.  
Thanks to my employer [VAS Software](https://vas.software) for allowing to give back to the comunity.