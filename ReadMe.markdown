# NCommon
NCommon is a light weight framework that provides implementations of commonly used design patterns for applications using a Domain Driven Design approach. 

## Building NCommon
Building NCommon is done via a [psake] (http://github.com/JamesKovacs/psake) script. Before running the psake build script, make sure you have Powershell 2.0 installed. 

> Import-Module .\psake.psm1
> Invoke-psake .\default.ps1

NCommon binaries are built and placed in an **out** directory under the root folder. 
