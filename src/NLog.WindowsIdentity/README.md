# NLog Windows Identity LayoutRenderer

NLog extensions for displaying [User Windows Identity](https://github.com/NLog/NLog/wiki/Windows-Identity-Layout-Renderer) and [target wrapper for user impersonation](https://github.com/NLog/NLog/wiki/ImpersonatingWrapper-target)

## Register Extension

NLog will only recognize type-alias `windows-identity` when loading from `NLog.config`-file, if having added extension to `NLog.config`-file:

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
