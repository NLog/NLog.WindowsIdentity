# NLog.WindowsIdentity
NLog extensions for displaying [User Windows Identity](https://github.com/NLog/NLog/wiki/Windows-Identity-Layout-Renderer) and [target wrapper for user impersonation](https://github.com/NLog/NLog/wiki/ImpersonatingWrapper-target)

[![Version](https://badge.fury.io/nu/NLog.WindowsIdentity.svg)](https://www.nuget.org/packages/NLog.WindowsIdentity)
[![AppVeyor](https://img.shields.io/appveyor/ci/nlog/NLog-WindowsIdentity/master.svg)](https://ci.appveyor.com/project/nlog/NLog-WindowsIdentity/branch/master)


### How to install

1) Install the package

    `Install-Package NLog.WindowsIdentity` or in your csproj:

    ```xml
    <PackageReference Include="NLog.WindowsIdentity" Version="5.*" />
    ```

2) Add to your nlog.config:

    ```xml
    <extensions>
        <add assembly="NLog.WindowsIdentity"/>
    </extensions>
    ```

### How to use WindowsIdentityLayoutRenderer

```xml
<nlog>
    <extensions>
        <add assembly="NLog.WindowsIdentity"/>
    </extensions>
    <targets>
        <target name="console" xsi:type="console" layout="${message}|User=${windows-identity}"  />
    </targets>
    <rules>
        <logger minLevel="Info" writeTo="console" />
    </rules>
</nlog>
```

### How to use ImpersonatingTargetWrapper

```xml
<nlog>
    <extensions>
        <add assembly="NLog.WindowsIdentity"/>
    </extensions>
    <targets>
        <target xsi:type="ImpersonatingWrapper" userName="xxx">
            <target name="console" xsi:type="console" layout="${message}|User=${windows-identity}"  />
        </target>
    </targets>
    <rules>
        <logger minLevel="Info" writeTo="console" />
    </rules>
</nlog>
```