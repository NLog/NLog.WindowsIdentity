# NLog.WindowsIdentity
NLog extensions for displaying [User Windows Identity](https://github.com/NLog/NLog/wiki/Windows-Identity-Layout-Renderer) and [target wrapper for user impersonation](https://github.com/NLog/NLog/wiki/ImpersonatingWrapper-target)

[![Version](https://badge.fury.io/nu/NLog.WindowsIdentity.svg)](https://www.nuget.org/packages/NLog.WindowsIdentity)
[![AppVeyor](https://img.shields.io/appveyor/ci/nlog/NLog-WindowsIdentity/master.svg)](https://ci.appveyor.com/project/nlog/NLog-WindowsIdentity/branch/master)


### How to install

1) Install the package

    `Install-Package NLog.WindowsIdentity` or in your csproj:

    ```xml
    <PackageReference Include="NLog.WindowsIdentity" Version="6.*" />
    ```

2) Add to your nlog.config:

    ```xml
    <extensions>
        <add assembly="NLog.WindowsIdentity"/>
    </extensions>
    ```

   Alternative register from code using [fluent configuration API](https://github.com/NLog/NLog/wiki/Fluent-Configuration-API):

    ```csharp
    LogManager.Setup().SetupExtensions(ext => {
        ext.RegisterTarget<NLog.Targets.Wrappers.ImpersonatingTargetWrapper>();
        ext.RegisterLayoutRenderer<NLog.LayoutRenderers.WindowsIdentityLayoutRenderer>();
    });
    ```

### Example of Windows Identity UserName
Example of `NLog.config`-file that outputs username from [Windows Identity](https://github.com/NLog/NLog/wiki/Windows-Identity-Layout-Renderer) :

```xml
<nlog>
    <extensions>
        <add assembly="NLog.WindowsIdentity"/>
    </extensions>
    <targets>
        <target name="console" xsi:type="console" layout="${message}|User=${windows-identity}" />
    </targets>
    <rules>
        <logger minLevel="Info" writeTo="console" />
    </rules>
</nlog>
```

### Example of Impersonating Windows Identity
Example of `NLog.config`-file that apply [ImpersonatingWrapper](https://github.com/NLog/NLog/wiki/ImpersonatingWrapper-target) :

```xml
<nlog>
    <extensions>
        <add assembly="NLog.WindowsIdentity"/>
    </extensions>
    <targets>
        <target name="userConsole" xsi:type="ImpersonatingWrapper" userName="xxx">
            <target name="console" xsi:type="console" layout="${message}|User=${windows-identity}" />
        </target>
    </targets>
    <rules>
        <logger minLevel="Info" writeTo="userConsole" />
    </rules>
</nlog>
```
