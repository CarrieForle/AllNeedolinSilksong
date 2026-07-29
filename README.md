# AllNeedolin

Show every needolin message all at once.

![](https://github.com/user-attachments/assets/510e0228-4027-4685-8865-df1accf0e029)
![](https://github.com/user-attachments/assets/174e1963-1112-474c-8b61-1df555665612)

## Install

Install BepinEx. Download the mod and unzip it to plugins folder.

```
.
└── BepinEx/
    └── plugins/
        └── CarrieForle-AllNeedolin/
            ├── ...
            ├── AllNeedolin.dll
            └── AllNeedolin.pdb
```

If you use a mod manager (e.g., r2modman), the above structure should be in a profile folder. If you installed BepinEx manually, it should be in the Silksong installation folder.

## Build

.NET 10 is required.

Create `SilksongPath.props` under `AllNeedolin`. Copy and paste the following text and edit as needed.

```xml
<Project>
  <PropertyGroup>
    <SilksongFolder>SilksongInstallPath</SilksongFolder>
    <!-- If you use a mod manager rather than manually installing BepInEx, this should be a profile directory for that mod manager. -->
    <SilksongPluginsFolder>$(SilksongFolder)/BepInEx/plugins</SilksongPluginsFolder>
  </PropertyGroup>
</Project>
```

```sh
dotnet build -c Release
```
