import { ConsoleCommand } from "../Models/ConsoleCommand.js";
import { Parameter } from "../Models/Parameter.js";
var commands = [
    new ConsoleCommand(`AddDscResourceProperty`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`AddDscResourcePropertyFromMetadata`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Add-NodeKeys`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`CheckResourceFound`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Compress-Archive`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Configuration`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ConvertFrom-SddlString`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ConvertTo-MOFInstance`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Disable-PSTrace`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Disable-PSWSManCombinedTrace`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Disable-WSManTrace`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Enable-PSTrace`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Enable-PSWSManCombinedTrace`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Enable-WSManTrace`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Expand-Archive`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Find-Command`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Find-DscResource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Find-Module`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Find-RoleCapability`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Find-Script`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Generate-VersionInfo`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-CompatibleVersionAddtionaPropertiesStr`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-ComplexResourceQualifier`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`GetCompositeResource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-ConfigurationErrorCount`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-DscResource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-DSCResourceModules`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-EncryptedPassword`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`GetImplementingModulePath`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-InnerMostErrorRecord`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-InstalledModule`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-InstalledScript`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-LogProperties`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`GetModule`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-MofInstanceName`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-MofInstanceText`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`GetPatterns`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PositionInfo`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSCurrentConfigurationNode`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSDefaultConfigurationDocument`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSMetaConfigDocumentInstVersionInfo`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSMetaConfigurationProcessed`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSRepository`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSTopConfigurationName`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PublicKeyFromFile`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PublicKeyFromStore`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`GetResourceFromKeyword`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`GetSyntax`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ImportCimAndScriptKeywordsFromModule`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ImportClassResourcesFromModule`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Initialize-ConfigurationRuntimeState`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Install-Module`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Install-Script`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`IsHiddenResource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`IsPatternMatched`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-DscChecksum`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-ScriptFileInfo`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Node`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`PSConsoleHostReadline`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Publish-Module`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Publish-Script`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ReadEnvironmentFile`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Register-PSRepository`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Save-Module`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Save-Script`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-LogProperties`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-NodeExclusiveResources`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-NodeManager`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-NodeResources`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-NodeResourceSource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-PSCurrentConfigurationNode`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-PSDefaultConfigurationDocument`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-PSMetaConfigDocInsProcessedBeforeMeta`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-PSMetaConfigVersionInfoV2`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-PSRepository`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-PSTopConfigurationName`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Start-Trace`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Stop-Trace`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`StrongConnect`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Test-ConflictingResources`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Test-ModuleReloadRequired`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Test-MofInstanceText`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Test-NodeManager`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Test-NodeResources`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Test-NodeResourceSource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Test-ScriptFileInfo`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ThrowError`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Uninstall-Module`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Uninstall-Script`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Unregister-PSRepository`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Update-ConfigurationDocumentRef`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Update-ConfigurationErrorCount`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Update-DependsOn`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Update-LocalConfigManager`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Update-Module`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Update-ModuleManifest`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Update-ModuleVersion`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Update-Script`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Update-ScriptFileInfo`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ValidateNoCircleInNodeResources`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ValidateNodeExclusiveResources`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ValidateNodeManager`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ValidateNodeResources`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ValidateNodeResourceSource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ValidateNoNameNodeResources`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ValidateUpdate-ConfigurationData`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`WriteFile`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Write-Log`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Write-MetaConfigFile`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Write-NodeMOFFile`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Add-Content`, [
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as "User01" or "Domain01\User01", or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Encoding`, `Specifies the encoding that this cmdlet applies to the content.

The acceptable values for this parameter are:

- Unknown

- String

- Unicode

- Byte

- BigEndianUnicode

- UTF8

- UTF7

- UTF32

- Ascii

- Default

- Oem

- BigEndianUTF32`, `FileSystemCmdletProviderEncoding`),
        new Parameter(`Exclude`, `Omits the specified items. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as "*.txt". Wildcards are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when retrieving the objects, rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Forces the command to run without asking for user confirmation.`, `SwitchParameter`),
        new Parameter(`Include`, `Adds only the specified items. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as "*.txt". Wildcards are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies the path to the items that receive the additional content. Unlike Path, the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`NoNewline`, `Indicates that this cmdlet does not add a new line/carriage return to the content.

The string representations of the input objects are concatenated to form the output. No spaces or newlines are inserted between the output strings. No newline is added after the last output string.`, `SwitchParameter`),
        new Parameter(`PassThru`, `Returns an object representing the added content. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies the path to the items that receive the additional content. Wildcards are permitted. If you specify multiple paths, use commas to separate the paths.`, `String[]`),
        new Parameter(`Stream`, `Specifies an alternative data stream for content. If the stream does not exist, this cmdlet creates it. Wildcard characters are not supported.

Stream is a dynamic parameter that the FileSystem provider adds to Add-Content . This parameter works only in file system drives.

You can use the Add-Content cmdlet to change the content of the Zone.Identifier alternate data stream. However, we do not recommend this as a way to eliminate security checks that block files that are downloaded from the Internet. If you verify that a downloaded file is safe, use the Unblock-File cmdlet.

This parameter was introduced in PowerShell 3.0.`, `String`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
        new Parameter(`Value`, `Specifies the content to be added. Type a quoted string, such as "This data is for internal use only", or specify an object that contains content, such as the DateTime object that Get-Date generates.

You cannot specify the contents of a file by typing its path, because the path is just a string, but you can use a Get-Content command to get the content and pass it to the Value parameter.`, `Object[]`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Adds content to the specified items, such as adding words to a file.`, `Add-Content [-Value] <Object[]> [-Confirm] [-Credential <PSCredential>] [-Encoding {Unknown | String | Unicode | Byte | BigEndianUnicode | UTF8 | UTF7 | UTF32 | Ascii | Default | Oem | BigEndianUTF32}] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-NoNewline] [-PassThru] [-Stream <String>] [-UseTransaction] [-WhatIf] [<CommonParameters>]

Add-Content [-Path] <String[]> [-Value] <Object[]> [-Confirm] [-Credential <PSCredential>] [-Encoding {Unknown | String | Unicode | Byte | BigEndianUnicode | UTF8 | UTF7 | UTF32 | Ascii | Default | Oem | BigEndianUTF32}] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-NoNewline] [-PassThru] [-Stream <String>] [-UseTransaction] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Add-History`, [
        new Parameter(`InputObject`, `Specifies an array of entries to add to the history as HistoryInfo object to the session history. You can use this parameter to submit a HistoryInfo object, such as the ones that are returned by the Get-History , Import-Clixml, or Import-Csv cmdlets, to Add-History .`, `PSObject[]`),
        new Parameter(`Passthru`, `Indicates that this cmdlet returns a history object for each history entry. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
    ], `Appends entries to the session history.`, `Add-History [[-InputObject] <PSObject[]>] [-Passthru] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Add-Member`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Add-Type`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Clear-Content`, [
        new Parameter(`Stream`, `Type a user name, such as "User01" or "Domain01\User01", or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `String`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as "User01" or "Domain01\User01", or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies, as a string array, strings that this cmdlet omits from the path to the content. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as "*.txt". Wildcards are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when retrieving the objects, rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Forces the command to run without asking for user confirmation.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies, as a string array, content that this cmdlet clears. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as "*.txt". Wildcards are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies the paths to the items from which content is deleted. Unlike the Path parameter, the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell having PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Path`, `Specifies the paths to the items from which content is deleted. Wildcards are permitted. The paths must be paths to items, not to containers. For example, you must specify a path to one or more files, not a path to a directory. Wildcards are permitted. This parameter is required, but the parameter name ("Path") is optional.`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Deletes the contents of an item, but does not delete the item.`, `Clear-Content [-Stream <String>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Clear-Content [-Path] <String[]> [-Stream <String>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Clear-History`, [
        new Parameter(`CommandLine`, `Specifies commands that this cmdlet deletes. If you enter more than one string, Clear-History deletes commands that have any of the strings.`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Count`, `Specifies the number of history entries that this cmdlet clears, starting with the oldest entry in the history.

If you use the Count and Id parameters in the same command, the cmdlet clears the number of entries specified by the Count parameter, starting with the entry specified by the Id parameter. For example, if Count is 10 and Id is 30, Clear-History clears items 21 through 30 inclusive.

If you use the Count and CommandLine parameters in the same command, Clear-History clears the number of entries specified by the Count parameter, starting with the entry specified by the CommandLine parameter.`, `Int32`),
        new Parameter(`Id`, `Specifies the history IDs of commands that this cmdlet deletes.

To find the history ID of a command, use the Get-History cmdlet.`, `Int32[]`),
        new Parameter(`Newest`, `Indicates that this cmdlet deletes the newest entries in the history. By default, Clear-History deletes the oldest entries in the history.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Deletes entries from the command history.`, `Clear-History [[-Count] <Int32>] [-CommandLine <String[]>] [-Confirm] [-Newest] [-WhatIf] [<CommonParameters>]

Clear-History [[-Id] <Int32[]>] [[-Count] <Int32>] [-Confirm] [-Newest] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Clear-Item`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies, as a string array, items to exclude. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when the cmdlet gets the objects, rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Indicates that the cmdlet clears items that cannot otherwise be changed, such as read- only aliases. The cmdlet cannot clear constants. Implementation varies from provider to provider. For more information, see about_Providers. The cmdlet cannot override security restrictions, even when the Force parameter is used.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies, as a string array, items to that this cmdlet clears. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies the path to the items being cleared. Unlike Path , the value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell wps_2 not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Path`, `Specifies the path to the items being cleared. Wildcards are permitted. This parameter is required, but the parameter name (Path) is optional.`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Clears the contents of an item, but does not delete the item.`, `Clear-Item [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Clear-Item [-Path] <String[]> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Clear-ItemProperty`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies, as a string array, an item or items that this cmdlet excludes. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as .txt or s . Wildcards are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when the cmdlet gets the objects rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Indicates that this cmdlet deletes properties from items that cannot otherwise be accessed by the user. Implementation varies from provider to provider. For more information, see about_Providers.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies, as a string array, an item or items that this cmdlet clears. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies the path to the property being cleared. Unlike the Path parameter, the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Name`, `Specifies the name of the property to be cleared, such as the name of a registry value. Wildcards are not permitted.`, `String`),
        new Parameter(`PassThru`, `Returns an object representing the item with which you are working. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies the path to the property being cleared. Wildcards are permitted.`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Clears the value of a property but does not delete the property.`, `Clear-ItemProperty [-Name] <String> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Clear-ItemProperty [-Path] <String[]> [-Name] <String> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Clear-Variable`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Compare-Object`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Connect-PSSession`, [
        new Parameter(`AllowRedirection`, `Indicates that this cmdlet allows redirection of this connection to an alternate URI.

When you use the ConnectionURI parameter, the remote destination can return an instruction to redirect to a different URI. By default, PowerShell does not redirect connections, but you can use this parameter to allow it to redirect the connection.

You can also limit the number of times the connection is redirected by changing the MaximumConnectionRedirectionCount session option value. Use the MaximumRedirection parameter of the New-PSSessionOption cmdlet or set the MaximumConnectionRedirectionCount property of the $PSSessionOption preference variable. The default value is 5.`, `SwitchParameter`),
        new Parameter(`ApplicationName`, `Specifies the name of an application. This cmdlet connects only to sessions that use the specified application.

Enter the application name segment of the connection URI. For example, in the following connection URI, the application name is WSMan: "http://localhost:5985/WSMAN". The application name of a session is stored in the Runspace.ConnectionInfo.AppName property of the session.

The value of this parameter is used to select and filter sessions. It does not change the application that the session uses.`, `String`),
        new Parameter(`Authentication`, `Specifies the mechanism that is used to authenticate user credentials in the command to reconnect to the disconnected session. The acceptable values for this parameter are:

- Default

- Basic

- Credssp

- Digest

- Kerberos

- Negotiate

- NegotiateWithImplicitCredential



The default value is Default.

For more information about the values of this parameter, see AuthenticationMechanism Enumeration (https://msdn.microsoft.com/library/system.management.automation.runspaces.authenticationmechanism)in the MSDN library.

Caution: Credential Security Support Provider (CredSSP) authentication, in which the user's credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.`, `AuthenticationMechanism`),
        new Parameter(`CertificateThumbprint`, `Specifies the digital public key certificate (X509) of a user account that has permission to connect to the disconnected session. Enter the certificate thumbprint of the certificate.

Certificates are used in client certificate-based authentication. They can be mapped only to local user accounts. They do not work with domain accounts.

To get a certificate thumbprint, use a Get-Item or Get-ChildItem command in the PowerShell Cert: drive.`, `String`),
        new Parameter(`ComputerName`, `Specifies the computers on which the disconnected sessions are stored. Sessions are stored on the computer that is at the server-side or receiving end of a connection. The default is the local computer.

Type the NetBIOS name, an IP address, or a fully qualified domain name of one computer. Wildcard characters are not permitted. To specify the local computer, type the computer name, localhost, or a dot (.)`, `String[]`),
        new Parameter(`ConfigurationName`, `Connects only to sessions that use the specified session configuration.

Enter a configuration name or the fully qualified resource URI for a session configuration. If you specify only the configuration name, the following schema URI is prepended: http://schemas.microsoft.com/powershell. The configuration name of a session is stored in the ConfigurationName property of the session.

The value of this parameter is used to select and filter sessions. It does not change the session configuration that the session uses.

For more information about session configurations, see about_Session_Configurations (About/about_Session_Configurations.md).`, `String`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`ConnectionUri`, `Specifies the URIs of the connection endpoints for the disconnected sessions.

The URI must be fully qualified. The format of this string is as follows:

"<Transport>://<ComputerName>:<Port>/<ApplicationName>"

The default value is as follows:

"http://localhost:5985/WSMAN"

If you do not specify a connection URI, you can use the UseSSL and Port parameters to specify the connection URI values.

Valid values for the Transport segment of the URI are HTTP and HTTPS. If you specify a connection URI with a Transport segment, but do not specify a port, the session is created with standards ports: 80 for HTTP and 443 for HTTPS. To use the default ports for PowerShell remoting, specify port 5985 for HTTP or 5986 for HTTPS.

If the destination computer redirects the connection to a different URI, PowerShell prevents the redirection unless you use the AllowRedirection parameter in the command.`, `Uri[]`),
        new Parameter(`Credential`, `Specifies a user account that has permission to connect to the disconnected session. The default is the current user.

Type a user name, such as User01 or Domain01\User01. Or, enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.`, `PSCredential`),
        new Parameter(`Id`, `Specifies the IDs of the disconnected sessions. The Id parameter works only when the disconnected session was previously connected to the current session.

This parameter is valid, but not effective, when the session is stored on the local computer, but was not connected to the current session.`, `Int32[]`),
        new Parameter(`InstanceId`, `Specifies the instance IDs of the disconnected sessions.

The instance ID is a GUID that uniquely identifies a PSSession on a local or remote computer.

The instance ID is stored in the InstanceID property of the PSSession .`, `Guid[]`),
        new Parameter(`Name`, `Specifies the friendly names of the disconnected sessions.`, `String[]`),
        new Parameter(`Port`, `Specifies the network port on the remote computer that is used to reconnect to the session. To connect to a remote computer, the remote computer must be listening on the port that the connection uses. The default ports are 5985, which is the WinRM port for HTTP, and 5986, which is the WinRM port for HTTPS.

Before using an alternate port, you must configure the WinRM listener on the remote computer to listen at that port. To configure the listener, type the following two commands at the PowerShell prompt:

"Remove-Item -Path WSMan:\Localhost\listener\listener* -Recurse"

"New-Item -Path WSMan:\Localhost\listener -Transport http -Address * -Port <port-number>"

Do not use the Port parameter unless you must. The port that is set in the command applies to all computers or sessions on which the command runs. An alternate port setting might prevent the command from running on all computers.`, `Int32`),
        new Parameter(`Session`, `Specifies the disconnected sessions. Enter a variable that contains the PSSession objects or a command that creates or gets the PSSession objects, such as a Get-PSSession command.`, `PSSession[]`),
        new Parameter(`SessionOption`, `Specifies advanced options for the session. Enter a SessionOption object, such as one that you create by using the New-PSSessionOption cmdlet, or a hash table in which the keys are session option names and the values are session option values.

The default values for the options are determined by the value of the $PSSessionOption preference variable, if it is set. Otherwise, the default values are established by options set in the session configuration.

The session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration.

For a description of the session options that includes the default values, see New-PSSessionOption. For information about the $PSSessionOption preference variable, see about_Preference_Variables (About/about_Preference_Variables.md). For more information about session configurations, see about_Session_Configurations (About/about_Session_Configurations.md).`, `PSSessionOption`),
        new Parameter(`ThrottleLimit`, `Specifies the maximum number of concurrent connections that can be established to run this command. If you omit this parameter or enter a value of 0, the default value, 32, is used.

The throttle limit applies only to the current command, not to the session or to the computer.`, `Int32`),
        new Parameter(`UseSSL`, `Indicates that this cmdlet uses the Secure Sockets Layer (SSL) protocol to connect to the disconnected session. By default, SSL is not used.

WS-Management encrypts all PowerShell content transmitted over the network. The UseSSL parameter is an additional protection that sends the data across an HTTPS connection instead of an HTTP connection.

If you use this parameter, but SSL is not available on the port that is used for the command, the command fails.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Reconnects to disconnected sessions.`, `Connect-PSSession [-ConnectionUri] <Uri[]> [-AllowRedirection] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Confirm] [-Credential <PSCredential>] [-Name <String[]>] [-SessionOption <PSSessionOption>] [-ThrottleLimit <Int32>] [-WhatIf] [<CommonParameters>]

Connect-PSSession [-ConnectionUri] <Uri[]> [-AllowRedirection] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Confirm] [-Credential <PSCredential>] -InstanceId <Guid[]> [-SessionOption <PSSessionOption>] [-ThrottleLimit <Int32>] [-WhatIf] [<CommonParameters>]

Connect-PSSession [-ApplicationName <String>] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] -ComputerName <String[]> [-ConfigurationName <String>] [-Confirm] [-Credential <PSCredential>] [-Name <String[]>] [-Port <Int32>] [-SessionOption <PSSessionOption>] [-ThrottleLimit <Int32>] [-UseSSL] [-WhatIf] [<CommonParameters>]

Connect-PSSession [-ApplicationName <String>] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] -ComputerName <String[]> [-ConfigurationName <String>] [-Confirm] [-Credential <PSCredential>] -InstanceId <Guid[]> [-Port <Int32>] [-SessionOption <PSSessionOption>] [-ThrottleLimit <Int32>] [-UseSSL] [-WhatIf] [<CommonParameters>]

Connect-PSSession [-Id] <Int32[]> [-Confirm] [-ThrottleLimit <Int32>] [-WhatIf] [<CommonParameters>]

Connect-PSSession [-Confirm] -InstanceId <Guid[]> [-ThrottleLimit <Int32>] [-WhatIf] [<CommonParameters>]

Connect-PSSession [-Confirm] -Name <String[]> [-ThrottleLimit <Int32>] [-WhatIf] [<CommonParameters>]

Connect-PSSession [-Session] <PSSession[]> [-Confirm] [-ThrottleLimit <Int32>] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Connect-WSMan`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ConvertFrom-Csv`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ConvertFrom-Json`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ConvertFrom-SecureString`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ConvertFrom-StringData`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Convert-Path`, [
        new Parameter(`LiteralPath`, `Specifies, as a string array, the path to be converted. The value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Path`, `Specifies the PowerShell path to be converted.`, `String[]`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Converts a path from a PowerShell path to a PowerShell provider path.`, `Convert-Path -LiteralPath <String[]> [-UseTransaction] [<CommonParameters>]

Convert-Path [-Path] <String[]> [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ConvertTo-Csv`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ConvertTo-Html`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ConvertTo-Json`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ConvertTo-SecureString`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ConvertTo-Xml`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Copy-Item`, [
        new Parameter(`Container`, `Indicates that this cmdlet preserves container objects during the copy operation.`, `SwitchParameter`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you are prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Destination`, `Specifies the path to the new location. To rename a copied item, include the new name in the value.`, `String`),
        new Parameter(`Exclude`, `Specifies, as a string array, an item or items that this cmdlet excludes from the operation. Wildcards are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when the cmdlet gets the objects, rather than have PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Indicates that this cmdlet copies items that cannot otherwise be changed, such as copying over a read-only file or alias.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies, as a string array, only those items upon which the cmdlet acts, excluding all others.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies a path to the item. The value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`PassThru`, `Returns an object representing the item with which you are working. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies, as a string array, the path to the items to copy.`, `String[]`),
        new Parameter(`Recurse`, `Indicates that this cmdlet performs a recursive copy.`, `SwitchParameter`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
        new Parameter(`FromSession`, `Specifies the PSSession object from which a remote file is being copied. When you use this parameter, the Path and LiteralPath parameters refer to the local path on the remote machine.`, `PSSession`),
        new Parameter(`ToSession`, `Specifies the PSSession object to which a remote file is being copied. When you use this parameter, the Path and LiteralPath parameters refer to the local path on the remote machine.`, `PSSession`),
    ], `Copies an item from one location to another.`, `Copy-Item [[-Destination] <String>] [-Container] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-PassThru] [-Recurse] [-Confirm] [-WhatIf] [-UseTransaction] [-FromSession <PSSession>] [-ToSession <PSSession>] [<CommonParameters>]

Copy-Item [-Path] <String[]> [[-Destination] <String>] [-Container] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-PassThru] [-Recurse] [-Confirm] [-WhatIf] [-UseTransaction] [-FromSession <PSSession>] [-ToSession <PSSession>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Copy-ItemProperty`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Destination`, `Specifies the path to the destination location.`, `String`),
        new Parameter(`Exclude`, `Specifies, as a string array, an item or items that this cmdlet excludes. Wildcards are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when this cmdlet gets the objects rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Forces the command to run without asking for user confirmation.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies, as a string array, an item or items upon which the cmdlet will act, excluding all others.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies a path to the item property. The value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Name`, `Specifies the name of the property to be copied.`, `String`),
        new Parameter(`PassThru`, `Returns an object representing the item with which you are working. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies, as a string array, the path to the property to be copied.`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Copies a property and value from a specified location to another location.`, `Copy-ItemProperty [-Destination] <String> [-Name] <String> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Copy-ItemProperty [-Path] <String[]> [-Destination] <String> [-Name] <String> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Debug-Job`, [
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Id`, `Specifies the ID number of a running job. To get the ID number of a job, run the Get-Job cmdlet.`, `Int32`),
        new Parameter(`InstanceId`, `Specifies the instance ID GUID of a running job. To get the InstanceId of a job, run the Get-Job cmdlet, piping the results into a Format- * cmdlet, as shown in the following example:

"Get-Job | Format-List -Property Id,Name,InstanceId,State"`, `Guid`),
        new Parameter(`Job`, `Specifies a running job object. The simplest way to use this parameter is to save the results of a Get-Job command that returns the running job that you want to debug in a variable, and then specify the variable as the value of this parameter.`, `Job`),
        new Parameter(`Name`, `Specifies a job by the friendly name of the job. When you start a job, you can specify a job name by adding the JobName parameter, in cmdlets such as Invoke-Command and Start-Job.`, `String`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Debugs a running background, remote, or Windows PowerShell Workflow job.`, `Debug-Job [-Id] <Int32> [-Confirm] [-WhatIf] [<CommonParameters>]

Debug-Job [-InstanceId] <Guid> [-Confirm] [-WhatIf] [<CommonParameters>]

Debug-Job [-Job] <Job> [-Confirm] [-WhatIf] [<CommonParameters>]

Debug-Job [-Name] <String> [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Debug-Process`, [
        new Parameter(`Id`, `Specifies the process IDs of the processes to be debugged. The Id parameter name is optional.

To find the process ID of a process, type "Get-Process".`, `Int32[]`),
        new Parameter(`InputObject`, `Specifies the process objects that represent processes to be debugged. Enter a variable that contains the process objects or a command that gets the process objects, such as the Get-Process cmdlet. You can also pipe process objects to this cmdlet.`, `Process[]`),
        new Parameter(`Name`, `Specifies the names of the processes to be debugged. If there is more than one process with the same name, this cmdlet attaches a debugger to all processes with that name. The Name parameter is optional.`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Debugs one or more processes running on the local computer.`, `Debug-Process [-Id] <Int32[]> [-Confirm] [-WhatIf] [<CommonParameters>]

Debug-Process -InputObject <Process[]> [-Confirm] [-WhatIf] [<CommonParameters>]

Debug-Process [-Name] <String[]> [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Debug-Runspace`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Disable-PSBreakpoint`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Disable-PSRemoting`, [
        new Parameter(`Confirm`, ``, `switch`),
        new Parameter(`Force`, ``, `switch`),
        new Parameter(`WhatIf`, ``, `switch`),
    ], `Disable-PSRemoting [-Force] [-WhatIf] [-Confirm] [<CommonParameters>]`, `syntaxItem                                                                                                    
----------                                                                                                    
{@{name=Disable-PSRemoting; CommonParameters=True; WorkflowCommonParameters=False; parameter=System.Object[]}}`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Disable-PSSessionConfiguration`, [
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Force`, `Forces the command to run without asking for user confirmation.`, `SwitchParameter`),
        new Parameter(`Name`, `Specifies an array of names of session configurations to disable. Enter one or more configuration names. Wildcard characters are permitted. You can also pipe a string that contains a configuration name or a session configuration object to Disable-PSSessionConfiguration .

If you omit this parameter, Disable-PSSessionConfiguration disables the Microsoft.PowerShell session configuration.`, `String[]`),
        new Parameter(`NoServiceRestart`, ``, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Disables session configurations on the local computer.`, `Disable-PSSessionConfiguration [[-Name] <String[]>] [-Confirm] [-Force] [-NoServiceRestart] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Disable-RunspaceDebug`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Disable-WSManCredSSP`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Disconnect-PSSession`, [
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Id`, `Specifies an array of IDs of sessions that this cmdlet disconnects. Type one or more IDs, separated by commas, or use the range operator (..) to specify a range of IDs.

To get the ID of a session, use the Get-PSSession cmdlet. The instance ID is stored in the ID property of the session.`, `Int32[]`),
        new Parameter(`IdleTimeoutSec`, `Changes the idle time-out value of the disconnected PSSession . Enter a value in seconds. The minimum value is 60 (1 minute).

The idle time-out determines how long the disconnected PSSession is maintained on the remote computer. When the time-out expires, the PSSession is deleted.

Disconnected PSSession objects are considered to be idle from the moment that they are disconnected, even if commands are running in the disconnected session.

The default value for the idle time-out of a session is set by the value of the IdleTimeoutMs property of the session configuration. The default value is 7200000 milliseconds (2 hours).

The value of this parameter takes precedence over the value of the IdleTimeout property of the $PSSessionOption preference variable and the default idle time-out value in the session configuration. However, this value cannot exceed the value of the MaxIdleTimeoutMs property of the session configuration. The default value of MaxIdleTimeoutMs is 12 hours (43200000 milliseconds).`, `Int32`),
        new Parameter(`InstanceId`, `Specifies an array of GUIDs of sessions that this cmdlet disconnects.

The instance ID is a GUID that uniquely identifies a session on a local or remote computer. The instance ID is unique, even across multiple sessions on multiple computers.

To get the instance ID of a session, use Get-PSSession . The instance ID is stored in the InstanceID property of the session.`, `Guid[]`),
        new Parameter(`Name`, `Specifies an array of friendly names of sessions that this cmdlet disconnects. Wildcard characters are permitted.

To get the friendly name of a session, use Get-PSSession . The friendly name is stored in the Name property of the session.`, `String[]`),
        new Parameter(`OutputBufferingMode`, `Specifies how command output is managed in the disconnected session when the output buffer is full. The default value is Block.

If the command in the disconnected session is returning output and the output buffer fills, the value of this parameter effectively determines whether the command continues to run while the session is disconnected. A value of Block suspends the command until the session is reconnected. A value of Drop allows the command to complete, although data might be lost. When using the Drop value, redirect the command output to a file on disk.

The acceptable values for this parameter are:

- Block. When the output buffer is full, execution is suspended until the buffer is clear.

- Drop. When the output buffer is full, execution continues. As new output is saved, the oldest output is discarded.

- None. No output buffering mode is specified. The value of the OutputBufferingMode property of the session configuration is used for the disconnected session.`, `OutputBufferingMode`),
        new Parameter(`Session`, `Specifies an array of sessions. Enter PSSession objects, such as those that the New-PSSession cmdlet returns. You can also pipe a PSSession object to Disconnect-PSSession. Get-PSSession can get all PSSession objects that end at a remote computer. These include PSSession objects that are disconnected and PSSession objects that are connected to other sessions on other computers. Disconnect-PSSession disconnects only PSSession that are connected to the current session. If you pipe other PSSession objects to Disconnect-PSSession , the Disconnect-PSSession command fails.`, `PSSession[]`),
        new Parameter(`ThrottleLimit`, `Sets the throttle limit for the Disconnect-PSSession command.

The throttle limit is the maximum number of concurrent connections that can be established to run this command. If you omit this parameter or enter a value of 0, the default value, 32, is used.

The throttle limit applies only to the current command, not to the session or to the computer.`, `Int32`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Disconnects from a session.`, `Disconnect-PSSession [-Id] <Int32[]> [-Confirm] [-IdleTimeoutSec <Int32>] [-OutputBufferingMode {None | Drop | Block}] [-ThrottleLimit <Int32>] [-WhatIf] [<CommonParameters>]

Disconnect-PSSession [-Confirm] [-IdleTimeoutSec <Int32>] -InstanceId <Guid[]> [-OutputBufferingMode {None | Drop | Block}] [-ThrottleLimit <Int32>] [-WhatIf] [<CommonParameters>]

Disconnect-PSSession [-Confirm] [-IdleTimeoutSec <Int32>] -Name <String[]> [-OutputBufferingMode {None | Drop | Block}] [-ThrottleLimit <Int32>] [-WhatIf] [<CommonParameters>]

Disconnect-PSSession [-Session] <PSSession[]> [-Confirm] [-IdleTimeoutSec <Int32>] [-OutputBufferingMode {None | Drop | Block}] [-ThrottleLimit <Int32>] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Disconnect-WSMan`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Enable-PSBreakpoint`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Enable-PSRemoting`, [
        new Parameter(`Confirm`, ``, `switch`),
        new Parameter(`Force`, ``, `switch`),
        new Parameter(`SkipNetworkProfileCheck`, ``, `switch`),
        new Parameter(`WhatIf`, ``, `switch`),
    ], `Enable-PSRemoting [-Force] [-SkipNetworkProfileCheck] [-WhatIf] [-Confirm] [<CommonParameters>]`, `syntaxItem                                                                                                   
----------                                                                                                   
{@{name=Enable-PSRemoting; CommonParameters=True; WorkflowCommonParameters=False; parameter=System.Object[]}}`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Enable-PSSessionConfiguration`, [
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Force`, `Indicates that the cmdlet does not prompt you for confirmation, and restarts the WinRM service without prompting. Restarting the service makes the configuration change effective.

To prevent a restart and suppress the restart prompt, use the NoServiceRestart parameter.`, `SwitchParameter`),
        new Parameter(`Name`, `Specifies the names of session configurations to enable. Enter one or more configuration names. Wildcard characters are permitted.

You can also pipe a string that contains a configuration name or a session configuration object to Enable-PSSessionConfiguration .

If you omit this parameter, Enable-PSSessionConfiguration enables the Microsoft.PowerShell session configuration.`, `String[]`),
        new Parameter(`NoServiceRestart`, `Indicates that the cmdlet does not restart the service.`, `SwitchParameter`),
        new Parameter(`SecurityDescriptorSddl`, `Specifies a security descriptor with which this cmdlet replaces the security descriptor on the session configuration.

If you omit this parameter, Enable-PSSessionConfiguration only deletes the deny all item from the security descriptor.`, `String`),
        new Parameter(`SkipNetworkProfileCheck`, `Indicates that this cmdlet enables the session configuration when the computer is on a public network. This parameter enables a firewall rule for public networks that allows remote access only from computers in the same local subnet. By default, Enable-PSSessionConfiguration fails on a public network.

This parameter is designed for client versions of the Windows operating system. By default, server versions of the Windows operating system have a local subnet firewall rule for public networks. However, if the local subnet firewall rule is disabled on a server version of the Windows operating system, this parameter re-enables it.

To remove the local subnet restriction and enable remote access from all locations on public networks, use the Set-NetFirewallRule cmdlet in the NetSecurity module. For more information, see Enable-PSRemoting.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Enables the session configurations on the local computer.`, `Enable-PSSessionConfiguration [[-Name] <String[]>] [-Confirm] [-Force] [-NoServiceRestart] [-SecurityDescriptorSddl <String>] [-SkipNetworkProfileCheck] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Enable-RunspaceDebug`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Enable-WSManCredSSP`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Enter-PSHostProcess`, [
        new Parameter(`AppDomainName`, ``, `String`),
        new Parameter(`HostProcessInfo`, ``, `PSHostProcessInfo`),
        new Parameter(`Id`, `Specifies a process by the process ID. To get a process ID, run the Get-Process cmdlet.`, `Int32`),
        new Parameter(`Name`, `Specifies a process by the process name. To get a process name, run the Get-Process cmdlet. You can also get process names from the Properties dialog box of a process in Task Manager.`, `String`),
        new Parameter(`Process`, `Specifies a process by the process object. The simplest way to use this parameter is to save the results of a Get-Process command that returns process that you want to enter in a variable, and then specify the variable as the value of this parameter.`, `Process`),
    ], `Connects to and enters into an interactive session with a local process.`, `Enter-PSHostProcess [-HostProcessInfo] <PSHostProcessInfo> [[-AppDomainName] <String>] [<CommonParameters>]

Enter-PSHostProcess [-Id] <Int32> [[-AppDomainName] <String>] [<CommonParameters>]

Enter-PSHostProcess [-Name] <String> [[-AppDomainName] <String>] [<CommonParameters>]

Enter-PSHostProcess [-Process] <Process> [[-AppDomainName] <String>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Enter-PSSession`, [
        new Parameter(`AllowRedirection`, `Allows redirection of this connection to an alternate Uniform Resource Identifier (URI). By default, redirection is not allowed.

When you use the ConnectionURI parameter, the remote destination can return an instruction to redirect to a different URI. By default, PowerShell does not redirect connections, but you can use this parameter to allow it to redirect the connection.

You can also limit the number of times the connection is redirected by changing the MaximumConnectionRedirectionCount session option value. Use the MaximumRedirection parameter of the New-PSSessionOption cmdlet or set the MaximumConnectionRedirectionCount property of the $PSSessionOption preference variable. The default value is 5.`, `SwitchParameter`),
        new Parameter(`ApplicationName`, `Specifies the application name segment of the connection URI. Use this parameter to specify the application name when you are not using the ConnectionURI parameter in the command.

The default value is the value of the $PSSessionApplicationName preference variable on the local computer. If this preference variable is not defined, the default value is WSMAN. This value is appropriate for most uses. For more information, see about_Preference_Variables.

The WinRM service uses the application name to select a listener to service the connection request. The value of this parameter should match the value of the URLPrefix property of a listener on the remote computer.`, `String`),
        new Parameter(`Authentication`, `Specifies the mechanism that is used to authenticate the user's credentials. The acceptable values for this parameter are:

- Default

- Basic

- Credssp

- Digest

- Kerberos

- Negotiate

- NegotiateWithImplicitCredential



The default value is Default.

CredSSP authentication is available only in Windows Vista, Windows Server 2008, and later versions of the Windows operating system.

For more information about the values of this parameter, see AuthenticationMechanism Enumeration (https://msdn.microsoft.com/library/system.management.automation.runspaces.authenticationmechanism)in the MSDN library.

Caution: Credential Security Support Provider (CredSSP) authentication, in which the user's credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.`, `AuthenticationMechanism`),
        new Parameter(`CertificateThumbprint`, `Specifies the digital public key certificate (X509) of a user account that has permission to perform this action. Enter the certificate thumbprint of the certificate.

Certificates are used in client certificate-based authentication. They can be mapped only to local user accounts; they do not work with domain accounts.

To get a certificate, use the Get-Item or Get-ChildItem command in the PowerShell Cert: drive.`, `String`),
        new Parameter(`ComputerName`, `Specifies a computer name. This cmdlet starts an interactive session with the specified remote computer. Enter only one computer name. The default is the local computer.

Type the NetBIOS name, the IP address, or the fully qualified domain name of the computer. You can also pipe a computer name to Enter-PSSession .

To use an IP address in the value of the ComputerName parameter, the command must include the Credential parameter. Also, the computer must be configured for HTTPS transport or the IP address of the remote computer must be included in the WinRM TrustedHosts list on the local computer. For instructions for adding a computer name to the TrustedHosts list, see "How to Add a Computer to the Trusted Host List" in about_Remote_Troubleshooting.

Note: In Windows Vista and later versions of the Windows operating system, to include the local computer in the value of the ComputerName parameter, you must start PowerShell with the Run as administrator option.`, `String`),
        new Parameter(`ConfigurationName`, `Specifies the session configuration that is used for the interactive session.

Enter a configuration name or the fully qualified resource URI for a session configuration. If you specify only the configuration name, the following schema URI is prepended: http://schemas.microsoft.com/powershell.

The session configuration for a session is located on the remote computer. If the specified session configuration does not exist on the remote computer, the command fails.

The default value is the value of the $PSSessionConfigurationName preference variable on the local computer. If this preference variable is not set, the default is Microsoft.PowerShell. For more information, see about_Preference_Variables.`, `String`),
        new Parameter(`ConnectionUri`, `Specifies a URI that defines the connection endpoint for the session. The URI must be fully qualified. The format of this string is as follows:

<Transport>://<ComputerName>:<Port>/<ApplicationName>

The default value is as follows:

"http://localhost:5985/WSMAN"

If you do not specify a ConnectionURI , you can use the UseSSL , ComputerName , Port , and ApplicationName parameters to specify the ConnectionURI values.

Valid values for the Transport segment of the URI are HTTP and HTTPS. If you specify a connection URI with a Transport segment, but do not specify a port, the session is created by using standards ports: 80 for HTTP and 443 for HTTPS. To use the default ports for PowerShell remoting, specify port 5985 for HTTP or 5986 for HTTPS.

If the destination computer redirects the connection to a different URI, PowerShell prevents the redirection unless you use the AllowRedirection parameter in the command.`, `Uri`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as "User01", "Domain01\User01", or "User@Domain.com", or enter a PSCredential object, such as one returned by the Get-Credential cmdlet.

When you type a user name, this cmdlet prompts you for a password.`, `PSCredential`),
        new Parameter(`EnableNetworkAccess`, `Indicates that this cmdlet adds an interactive security token to loopback sessions. The interactive token lets you run commands in the loopback session that get data from other computers. For example, you can run a command in the session that copies XML files from a remote computer to the local computer.

A loopback session is a PSSession that originates and ends on the same computer. To create a loopback session, omit the ComputerName parameter or set its value to . (dot), localhost, or the name of the local computer.

By default, loopback sessions are created by using a network token, which might not provide sufficient permission to authenticate to remote computers.

The EnableNetworkAccess parameter is effective only in loopback sessions. If you use EnableNetworkAccess when you create a session on a remote computer, the command succeeds, but the parameter is ignored.

You can also allow remote access in a loopback session by using the CredSSP value of the Authentication parameter, which delegates the session credentials to other computers.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`HostName`, `Specifies a computer name for a Secure Shell (SSH) based connection. This is similar to the ComputerName parameter except that the connection to the remote computer is made using SSH rather than Windows WinRM.

This parameter was introduced in PowerShell 6.0.`, `String`),
        new Parameter(`Id`, `Specifies the ID of an existing session. Enter-PSSession uses the specified session for the interactive session.

To find the ID of a session, use the Get-PSSession cmdlet.`, `Int32`),
        new Parameter(`InstanceId`, `Specifies the instance ID of an existing session. Enter-PSSession uses the specified session for the interactive session.

The instance ID is a GUID. To find the instance ID of a session, use the Get-PSSession cmdlet. You can also use the Session , Name , or ID parameters to specify an existing session. Or, you can use the ComputerName parameter to start a temporary session.`, `Guid`),
        new Parameter(`KeyFilePath`, `Specifies a key file path used by Secure Shell (SSH) to authenticate a user on a remote computer.

SSH allows user authentication to be performed via private/public keys as an alternative to basic password authentication. If the remote computer is configured for key authentication then this parameter can be used to provide the key that identifies the user.

This parameter was introduced in PowerShell 6.0.`, `String`),
        new Parameter(`Name`, `Specifies the friendly name of an existing session. Enter-PSSession uses the specified session for the interactive session.

If the name that you specify matches more than one session, the command fails. You can also use the Session , InstanceID , or ID parameters to specify an existing session. Or, you can use the ComputerName parameter to start a temporary session.

To establish a friendly name for a session, use the Name parameter of the New-PSSession cmdlet.`, `String`),
        new Parameter(`Port`, `Specifies the network port on the remote computer that is used for this command.

In PowerShell 6.0 this parameter was inlcuded in the HostName parameter set which supports Secure Shell (SSH) connections. WinRM (ComputerName parameter set) To connect to a remote computer, the remote computer must be listening on the port that the connection uses. The default ports are 5985, which is the WinRM port for HTTP, and 5986, which is the WinRM port for HTTPS.

Before using an alternate port, you must configure the WinRM listener on the remote computer to listen at that port. Use the following commands to configure the listener:

"1. winrm delete winrm/config/listener?Address=*+Transport=HTTP"

"2. winrm create winrm/config/listener?Address=*+Transport=HTTP @{Port="<port-number>"}"

Do not use the Port parameter unless you must. The port setting in the command applies to all computers or sessions on which the command runs. An alternate port setting might prevent the command from running on all computers. SSH (HostName parameter set) To connect to a remote computer, the remote computer must be configured with the SSH service (SSHD) and must be listening on the port that the connection uses. The default port for SSH is 22.`, `Int32`),
        new Parameter(`Session`, `Specifies a Windows PowerShell session ( PSSession ) to use for the interactive session. This parameter takes a session object. You can also use the Name , InstanceID , or ID parameters to specify a PSSession .

Enter a variable that contains a session object or a command that creates or gets a session object, such as a New-PSSession or Get-PSSession command. You can also pipe a session object to Enter-PSSession . You can submit only one PSSession by using this parameter. If you enter a variable that contains more than one PSSession , the command fails.

When you use Exit-PSSession or the EXIT keyword, the interactive session ends, but the PSSession that you created remains open and available for use.`, `PSSession`),
        new Parameter(`SessionOption`, `Sets advanced options for the session. Enter a SessionOption object, such as one that you create by using the New-PSSessionOption cmdlet, or a hash table in which the keys are session option names and the values are session option values.

The default values for the options are determined by the value of the $PSSessionOption preference variable, if it is set. Otherwise, the default values are established by options set in the session configuration.

The session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration.

For a description of the session options, including the default values, see New-PSSessionOption . For information about the $PSSessionOption preference variable, see about_Preference_Variables (About/about_Preference_Variables.md). For more information about session configurations, see about_Session_Configurations (About/about_Session_Configurations.md).`, `PSSessionOption`),
        new Parameter(`SSHTransport`, `Indicates that the remote connection is established using Secure Shell (SSH).

By default PowerShell uses Windows WinRM to connect to a remote computer. This switch forces PowerShell to use the HostName parameter set for establishing an SSH based remote connection.

This parameter was introduced in PowerShell 6.0.`, `SwitchParameter`),
        new Parameter(`UserName`, `Specifies the user name for the account used to create a session on the remote computer. User authentication method will depend on how Secure Shell (SSH) is configured on the remote computer.

If SSH is configured for basic password authentication then you will be prompted for the user password.

If SSH is configured for key based user authentication then a key file path can be provided via the KeyFilePath parameter and no password prompt will occur. Note that if the client user key file is located in an SSH known location then the KeyFilePath parameter is not needed for key based authentication, and user authentication will occur automatically based on the user name. See SSH documentation about key based user authentication for more information.

This is not a required parameter.  If no UserName parameter is specified then the current log on user name is used for the connection.

This parameter was introduced in PowerShell 6.0.`, `String`),
        new Parameter(`UseSSL`, `Indicates that this cmdlet uses the Secure Sockets Layer (SSL) protocol to establish a connection to the remote computer. By default, SSL is not used.

WS-Management encrypts all Windows PowerShell content transmitted over the network. The UseSSL parameter is an additional protection that sends the data across an HTTPS connection instead of an HTTP connection.

If you use this parameter, but SSL is not available on the port that is used for the command, the command fails.`, `SwitchParameter`),
        new Parameter(`VMName`, `Specifies the name of a virtual machine.`, `String`),
        new Parameter(`ContainerId`, `Specifies the ID of a container.`, `String`),
        new Parameter(`RunAsAdministrator`, `Indicates that the PSSession runs as administrator.`, `SwitchParameter`),
        new Parameter(`VMId`, `Specifies the ID of a virtual machine.`, `Guid`),
    ], `Starts an interactive session with a remote computer.`, `Enter-PSSession [[-ConnectionUri] <Uri>] [-AllowRedirection] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Credential <PSCredential>] [-EnableNetworkAccess] [-SessionOption <PSSessionOption>] [<CommonParameters>]

Enter-PSSession [-ComputerName] <String> [-ApplicationName <String>] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Credential <PSCredential>] [-EnableNetworkAccess] [-Port <Int32>] [-SessionOption <PSSessionOption>] [-UseSSL] [<CommonParameters>]

Enter-PSSession [-ConfigurationName <String>] -Credential <PSCredential> -VMId <Guid> [<CommonParameters>]

Enter-PSSession [-VMName] <String> [-ConfigurationName <String>] -Credential <PSCredential> [<CommonParameters>]

Enter-PSSession [-ConfigurationName <String>] -ContainerId <String> [-RunAsAdministrator] [<CommonParameters>]

Enter-PSSession [-HostName] <String> [-KeyFilePath <String>] [-Port <Int32>] [-SSHTransport] [-UserName <String>] [<CommonParameters>]

Enter-PSSession [[-Id] <Int32>] [<CommonParameters>]

Enter-PSSession [-InstanceId <Guid>] [<CommonParameters>]

Enter-PSSession [-Name <String>] [<CommonParameters>]

Enter-PSSession [[-Session] <PSSession>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Exit-PSHostProcess`, [], `Closes an interactive session with a local process.`, `Exit-PSHostProcess [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Exit-PSSession`, [], `Ends an interactive session with a remote computer.`, `Exit-PSSession [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Export-Alias`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Export-BinaryMiLog`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Export-Clixml`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Export-Csv`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Export-FormatData`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Export-ModuleMember`, [
        new Parameter(`Alias`, `Specifies the aliases that are exported from the script module file. Enter the alias names. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Cmdlet`, `Specifies the cmdlets that are exported from the script module file. Enter the cmdlet names. Wildcard characters are permitted.

You cannot create cmdlets in a script module file, but you can import cmdlets from a binary module into a script module and re-export them from the script module.`, `String[]`),
        new Parameter(`Function`, `Specifies the functions that are exported from the script module file. Enter the function names. Wildcard characters are permitted. You can also pipe function name strings to Export-ModuleMember .`, `String[]`),
        new Parameter(`Variable`, `Specifies the variables that are exported from the script module file. Enter the variable names, without a dollar sign. Wildcard characters are permitted.`, `String[]`),
    ], `Specifies the module members that are exported.`, `Export-ModuleMember [[-Function] <String[]>] [-Alias <String[]>] [-Cmdlet <String[]>] [-Variable <String[]>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Export-PSSession`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Find-Package`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Find-PackageProvider`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`ForEach-Object`, [
        new Parameter(`ArgumentList`, `Specifies an array of arguments to a method call.

This parameter was introduced in Windows PowerShell 3.0.`, `Object[]`),
        new Parameter(`Begin`, `Specifies a script block that runs before this cmdlet processes any input objects.`, `ScriptBlock`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`End`, `Specifies a script block that runs after this cmdlet processes all input objects.`, `ScriptBlock`),
        new Parameter(`InputObject`, `Specifies the input objects. "ForEach-Object" runs the script block or operation statement on each input object. Enter a variable that contains the objects, or type a command or expression that gets the objects.

When you use the InputObject parameter with "ForEach-Object", instead of piping command results to "ForEach-Object", the InputObject value is treated as a single object. This is true even if the value is a collection that is the result of a command, such as "-InputObject (Get-Process)". Because InputObject cannot return individual properties from an array or collection of objects, we recommend that if you use "ForEach-Object" to perform operations on a collection of objects for those objects that have specific values in defined properties, you use "ForEach-Object" in the pipeline, as shown in the examples in this topic.`, `PSObject`),
        new Parameter(`MemberName`, `Specifies the property to get or the method to call.

Wildcard characters are permitted, but work only if the resulting string resolves to a unique value. If, for example, you run "Get-Process | ForEach -MemberName Name", and more than one member exists with a name that contains the string Name, such as the ProcessName and Name * properties, the command fails.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`Process`, `Specifies the operation that is performed on each input object. Enter a script block that describes the operation.`, `ScriptBlock[]`),
        new Parameter(`RemainingScripts`, `Specifies all script blocks that are not taken by the Process parameter.

This parameter was introduced in Windows PowerShell 3.0.`, `ScriptBlock[]`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Performs an operation against each item in a collection of input objects.`, `ForEach-Object [-MemberName] <String> [-ArgumentList <Object[]>] [-Confirm] [-InputObject <PSObject>] [-WhatIf] [<CommonParameters>]

ForEach-Object [-Process] <ScriptBlock[]> [-Begin <ScriptBlock>] [-Confirm] [-End <ScriptBlock>] [-InputObject <PSObject>] [-RemainingScripts <ScriptBlock[]>] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Format-Custom`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Format-Hex`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Format-List`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Format-Table`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Format-Wide`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Acl`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Alias`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-AuthenticodeSignature`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-ChildItem`, [
        new Parameter(`Attributes`, `Gets files and folders with the specified attributes. This parameter supports all attributes and lets you specify complex combinations of attributes.

For example, to get non-system files (not directories) that are encrypted or compressed, type:



Get-ChildItem -Attributes !Directory+!System+Encrypted, !Directory+!System+Compressed



To find files and folders with commonly used attributes, you can use the "-Attributes" parameter, or the "-Directory", "-File", "-Hidden", "-ReadOnly", and "-System" switch parameters.

The "-Attributes" parameter supports the following attributes:

- Archive

- Compressed

- Device

- Directory

- Encrypted

- Hidden

- IntegrityStream

- Normal

- NoScrubData

- NotContentIndexed

- Offline

- ReadOnly

- ReparsePoint

- SparseFile

- System

- Temporary



For a description of these attributes, see the FileAttributes Enumeration (http://go.microsoft.com/fwlink/?LinkId=201508).

Use the following operators to combine attributes:

- "!"   (NOT)

- "+"   (AND)

- ","   (OR)



No spaces are permitted between an operator and its attribute. However, spaces are permitted before commas.

You can use the following abbreviations for commonly used attributes:

- "D"   (Directory)

- "H"   (Hidden)

- "R"   (Read-only)

- "S"   (System)`, `System.Management.Automation.FlagsExpression^1[System.IO.FileAttributes]`),
        new Parameter(`Depth`, `This parameter, added in Windows Powershell 5.0 enables you to control the depth of recursion. You use both the "-Recurse" and the "-Depth" parameter to limit the recursion.`, `UInt32`),
        new Parameter(`Directory`, `Gets directories (folders).

To get only directories, use the "-Directory" parameter and omit the "-File" parameter. To exclude directories, use the "-File" parameter and omit the "-Directory" parameter, or use the "-Attributes" parameter.

To get directories, use the Directory parameter, its ""ad"" alias, or the Directory attribute of the "-Attributes" parameter.`, `SwitchParameter`),
        new Parameter(`Exclude`, `Omits the specified items. The value of this parameter qualifies the "-Path" parameter. Enter a path element or pattern, such as "*.txt". Wildcards are permitted.`, `String[]`),
        new Parameter(`File`, `Gets files.

To get only files, use the "-File" parameter and omit the Directory parameter. To exclude files, use the "-Directory" parameter and omit the "-File" parameter, or use the "-Attributes" parameter.

To get files, use the File parameter, its ""af"" alias, or the File value of the "-Attributes" parameter.`, `SwitchParameter`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the "-Path" parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when retrieving the objects, rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`FollowSymlink`, `By default, the "Get-ChildItem" cmdlet displays symbolic links to directories found during recursion, but does not recurse into them. Use the FollowSymlink switch to search the directories that those symbolic links target. The FollowSymlink is a dynamic parameter and it is supported only in the FileSystem provider.`, `SwitchParameter`),
        new Parameter(`Force`, `Allows the cmdlet to get items that cannot otherwise not be accessed by the user, such as hidden or system files. Implementation varies among providers.

For more information, see about_Provider (../Microsoft.PowerShell.Core/About/about_Providers.md).

Even when using the "-Force" parameter, the cmdlet cannot override security restrictions.`, `SwitchParameter`),
        new Parameter(`Hidden`, `Gets only hidden files and directories (folders).  By default, "Get-ChildItem" gets only non-hidden items, but you can use the "-Force" parameter to include hidden items in the results.

To get only hidden items, use the "-Hidden" parameter, its ""h"" or ""ah"" aliases, or the Hidden value of the "-Attributes" parameter. To exclude hidden items, omit the "-Hidden" parameter or use the "-Attributes" parameter.`, `SwitchParameter`),
        new Parameter(`Include`, `Gets only the specified items. The value of this parameter qualifies the "-Path" parameter. Enter a path element or pattern, such as "*.txt". Wildcards are permitted.

The "-Include" parameter is effective only when the command includes the "-Recurse" parameter or the path leads to the contents of a directory, such as C:\Windows\ , where the "" "" wildcard character specifies the contents of the C:\Windows directory.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies a path to one or more locations. Unlike the "-Path" parameter, the value of the "-LiteralPath" parameter is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Name`, `Gets only the names of the items in the locations. If you pipe the output of this command to another command, only the item names are sent.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies a path to one or more locations. Wildcards are permitted. The default location is the current directory (".").`, `String[]`),
        new Parameter(`ReadOnly`, `Gets only read-only files and directories (folders).

To get only read-only items, use the "-ReadOnly" parameter, its ""ar"" alias, or the ReadOnly value of the "-Attributes" parameter. To exclude read-only items, use the "-Attributes" parameter.`, `SwitchParameter`),
        new Parameter(`Recurse`, `Gets the items in the specified locations and in all child items of the locations.`, `SwitchParameter`),
        new Parameter(`System`, `Gets only system files and directories (folders).

To get only system files and folders, use the "-System" parameter, its ""as"" alias, or the System value of the "-Attributes" parameter. To exclude system files and folders, use the "-Attributes" parameter.`, `SwitchParameter`),
    ], `Gets the items and child items in one or more specified locations.`, `Get-ChildItem [[-Filter] <String>] [-Attributes {ReadOnly | Hidden | System | Directory | Archive | Device | Normal | Temporary | SparseFile | ReparsePoint | Compressed | Offline | NotContentIndexed | Encrypted | IntegrityStream | NoScrubData}] [-Depth <UInt32>] [-Directory] [-Exclude <String[]>] [-File] [-FollowSymlink] [-Force] [-Hidden] [-Include <String[]>] -LiteralPath <String[]> [-Name] [-ReadOnly] [-Recurse] [-System] [<CommonParameters>]

Get-ChildItem [[-Path] <String[]>] [[-Filter] <String>] [-Attributes {ReadOnly | Hidden | System | Directory | Archive | Device | Normal | Temporary | SparseFile | ReparsePoint | Compressed | Offline | NotContentIndexed | Encrypted | IntegrityStream | NoScrubData}] [-Depth <UInt32>] [-Directory] [-Exclude <String[]>] [-File] [-FollowSymlink] [-Force] [-Hidden] [-Include <String[]>] [-Name] [-ReadOnly] [-Recurse] [-System] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-CimAssociatedInstance`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-CimClass`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-CimInstance`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-CimSession`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-CmsMessage`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Command`, [
        new Parameter(`All`, `Indicates that this cmdlet gets all commands, including commands of the same type that have the same name. By default, Get-Command gets only the commands that run when you type the command name.

For more information about the method that PowerShell uses to select the command to run when multiple commands have the same name, see about_Command_Precedence (About/about_Command_Precedence.md)in the TechNet library. For information about module-qualified command names and running commands that do not run by default because of a name conflict, see about_Modules (About/about_Modules.md).

This parameter was introduced in Windows PowerShell 3.0.

In Windows PowerShell 2.0, Get-Command gets all commands by default.`, `SwitchParameter`),
        new Parameter(`ArgumentList`, `Specifies an array of arguments. This cmdlet gets information about a cmdlet or function when it is used with the specified parameters ("arguments"). The alias for ArgumentList is Args .

To detect dynamic parameters that are available only when certain other parameters are used, set the value of ArgumentList to the parameters that trigger the dynamic parameters.

To detect the dynamic parameters that a provider adds to a cmdlet, set the value of the ArgumentList parameter to a path in the provider drive, such as WSMan:, HKLM:, or Cert:. When the command is a PowerShell provider cmdlet, enter only one path in each command. The provider cmdlets return only the dynamic parameters for the first path the value of ArgumentList . For information about the provider cmdlets, see about_Providers (About/about_Providers.md).`, `Object[]`),
        new Parameter(`CommandType`, `Specifies the  types of commands that this cmdlet gets. Enter one or more command types. Use CommandType or its alias, Type . By default, Get-Command gets all cmdlets, functions, and aliases.

The acceptable values for this parameter are:

- Alias. Gets the aliases of all PowerShell commands. For more information, see about_Aliases.

- All. Gets all command types. This parameter value is the equivalent of "Get-Command *".

- Application. Gets non-Windows-PowerShell files in paths listed in the Path environment variable ($env:path), including .txt, .exe, and .dll files. For more information about the Path environment variable, see about_Environment_Variables. - Cmdlet. Gets all cmdlets.

- ExternalScript. Gets all .ps1 files in the paths listed in the Path environment variable ($env:path). - Filter and Function. Gets all PowerShell advanced and simple functions and filters.

- Script. Gets all script blocks. To get PowerShell scripts (.ps1 files), use the ExternalScript value.`, `CommandTypes`),
        new Parameter(`FullyQualifiedModule`, `Specifies modules with names that are specified in the form of ModuleSpecification objects, described in the Remarks section of ModuleSpecification Constructor (Hashtable) (https://msdn.microsoft.com/library/jj136290)in the MSDN library. For example, the FullyQualifiedModule parameter accepts a module name that is specified in the format @{ModuleName = "modulename"; ModuleVersion = "version_number"} or @{ModuleName = "modulename"; ModuleVersion = "version_number"; Guid = "GUID"}. ModuleName and ModuleVersion are required, but Guid is optional.

You cannot specify the FullyQualifiedModule parameter in the same command as a Module parameter. The two parameters are mutually exclusive.`, `ModuleSpecification[]`),
        new Parameter(`ListImported`, `Indicates that this cmdlet gets only commands in the current session.

Starting in Windows PowerShell 3.0, by default, Get-Command gets all installed commands, including, but not limited to, the commands in the current session. In Windows PowerShell 2.0, it gets only commands in the current session.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`Module`, `Specifies an array of modules. This cmdlet gets the commands that came from the specified modules or snap-ins. Enter the names of modules or snap-ins, or enter snap-in or module objects.

This parameter takes string values, but the value of this parameter can also be a PSModuleInfo or PSSnapinInfo object, such as the objects that the Get-Module, Get-PSSnapin, and Import-PSSession cmdlets return.

You can refer to this parameter by its name, Module , or by its alias, PSSnapin . The parameter name that you choose has no effect on the command output.`, `String[]`),
        new Parameter(`Name`, `Specifies an array of names. This cmdlet gets only commands that have the specified name. Enter a name or name pattern. Wildcard characters are permitted.

To get commands that have the same name, use the All parameter. When two commands have the same name, by default, Get-Command gets the command that runs when you type the command name.`, `String[]`),
        new Parameter(`Noun`, `Specifies an array of command nouns. This cmdlet gets commands, which include cmdlets, functions, and aliases, that have names that include the specified noun. Enter one or more nouns or noun patterns. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`ParameterName`, `Specifies an array of parameter names. This cmdlet gets commands in the session that have the specified parameters. Enter parameter names or parameter aliases. Wildcard characters are supported.

The ParameterName and ParameterType parameters search only commands in the current session.

This parameter was introduced in Windows PowerShell 3.0.`, `String[]`),
        new Parameter(`ParameterType`, `Specifies an array of parameter names. This cmdlet gets commands in the session that have parameters of the specified type. Enter the full name or partial name of a parameter type. Wildcard characters are supported.

The ParameterName and ParameterType parameters search only commands in the current session.

This parameter was introduced in Windows PowerShell 3.0.`, `PSTypeName[]`),
        new Parameter(`ShowCommandInfo`, `Indicates that this cmdlet displays command information.

For more information about the method that PowerShell uses to select the command to run when multiple commands have the same name, see about_Command_Precedence (About/about_Command_Precedence.md). For information about module-qualified command names and running commands that do not run by default because of a name conflict, see about_Modules (About/about_Modules.md).

This parameter was introduced in Windows PowerShell 3.0.

In Windows PowerShell 2.0, Get-Command gets all commands by default.`, `SwitchParameter`),
        new Parameter(`Syntax`, `Indicates that this cmdlet gets only the following specified data about the command:

- Aliases. Gets the standard name.

- Cmdlets. Gets the syntax.

- Functions and filters. Gets the function definition.

- Scripts and applications or files. Gets the path and filename.`, `SwitchParameter`),
        new Parameter(`TotalCount`, `Specifies the number of commands to get. You can use this parameter to limit the output of a command.`, `Int32`),
        new Parameter(`Verb`, `Specifies an array of command verbs. This cmdlet gets commands, which include cmdlets, functions, and aliases, that have names that include the specified verb. Enter one or more verbs or verb patterns. Wildcard characters are permitted.`, `String[]`),
    ], `Gets all commands.`, `Get-Command [[-Name] <String[]>] [[-ArgumentList] <Object[]>] [-All] [-CommandType {Alias | Function | Filter | Cmdlet | ExternalScript | Application | Script | Configuration | All}] [-FullyQualifiedModule <ModuleSpecification[]>] [-ListImported] [-Module <String[]>] [-ParameterName <String[]>] [-ParameterType <PSTypeName[]>] [-ShowCommandInfo] [-Syntax] [-TotalCount <Int32>] [<CommonParameters>]

Get-Command [[-ArgumentList] <Object[]>] [-All] [-FullyQualifiedModule <ModuleSpecification[]>] [-ListImported] [-Module <String[]>] [-Noun <String[]>] [-ParameterName <String[]>] [-ParameterType <PSTypeName[]>] [-ShowCommandInfo] [-Syntax] [-TotalCount <Int32>] [-Verb <String[]>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-ComputerInfo`, [
        new Parameter(`Property`, `Specifies, as a string array, the computer properties in which this cmdlet displays.`, `String[]`),
    ], `Gets a consolidated object of system and operating system properties.`, `Get-ComputerInfo [[-Property] <String[]>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Content`, [
        new Parameter(`Encoding`, `This parameter is not supported by any providers that are installed with PowerShell.`, `FileSystemCmdletProviderEncoding`),
        new Parameter(`Delimiter`, `This parameter is not supported by any providers that are installed with PowerShell.`, `String`),
        new Parameter(`Force`, `Forces the command to run without asking for user confirmation.`, `SwitchParameter`),
        new Parameter(`Raw`, `This parameter is not supported by any providers that are installed with PowerShell.`, `SwitchParameter`),
        new Parameter(`Wait`, `This parameter is not supported by any providers that are installed with PowerShell.`, `SwitchParameter`),
        new Parameter(`Stream`, `This parameter is not supported by any providers that are installed with PowerShell.`, `String`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers that are installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies, as a string array, the item or items that this cmdlet omits when performing the operation. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when this cmdlet gets the objects, rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Include`, `Specifies, as a string array, the item or items that this cmdlet includes in the operation. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies the path to an item. Unlike the Path parameter, the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Path`, `Specifies the path to an item. Get-Content gets the content of the item. Wildcards are permitted.`, `String[]`),
        new Parameter(`ReadCount`, `Specifies how many lines of content are sent through the pipeline at a time. The default value is 1. A value of 0 (zero) sends all of the content at one time.

This parameter does not change the content displayed, but it does affect the time it takes to display the content. As the value of ReadCount increases, the time it takes to return the first line increases, but the total time for the operation decreases. This can make a perceptible difference in very large items.`, `Int64`),
        new Parameter(`TotalCount`, `Specifies the number of lines from the beginning of a file or other item. The default is -1 (all lines).

You can use the TotalCount parameter name or its aliases, First or Head.`, `Int64`),
        new Parameter(`Tail`, `Specifies the number of lines from the end of a file or other item.

This parameter was introduced in Windows PowerShell 3.0.

You can use the Tail parameter name or its alias, Last.`, `Int32`),
    ], `Gets the content of the item at the specified location.`, `Get-Content [-Encoding {Unknown | String | Unicode | Byte | BigEndianUnicode | UTF8 | UTF7 | UTF32 | Ascii | Default | Oem | BigEndianUTF32}] [-Delimiter <String>] [-Force] [-Raw] [-Wait] [-Stream <String>] [-UseTransaction] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Include <String[]>] -LiteralPath <String[]> [-ReadCount <Int64>] [-TotalCount <Int64>] [-Tail <Int32>] [<CommonParameters>]

Get-Content [-Path] <String[]> [-Encoding {Unknown | String | Unicode | Byte | BigEndianUnicode | UTF8 | UTF7 | UTF32 | Ascii | Default | Oem | BigEndianUTF32}] [-Delimiter <String>] [-Force] [-Raw] [-Wait] [-Stream <String>] [-UseTransaction] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Include <String[]>] [-ReadCount <Int64>] [-TotalCount <Int64>] [-Tail <Int32>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Credential`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Culture`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Date`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Event`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-EventSubscriber`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-ExecutionPolicy`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-FileHash`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-FormatData`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Help`, [
        new Parameter(`Category`, `Displays help only for items in the specified category and their aliases. The acceptable values for this parameter are:

- Alias

- Cmdlet

- Provider

- General

- FAQ

- Glossary

- HelpFile

- ScriptCommand

- Function

- Filter

- ExternalScript

- All

- DefaultHelp

- Workflow

- DscResource

- Class

- Configuration



Conceptual topics are in the HelpFile category.`, `String[]`),
        new Parameter(`Component`, `Displays commands with the specified component value, such as "Exchange." Enter a component name. Wildcard characters are permitted.

This parameter has no effect on displays of conceptual ("About_") help.`, `String[]`),
        new Parameter(`Detailed`, `Adds parameter descriptions and examples to the basic help display.

This parameter is effective only when help files are for the command are installed on the computer. It has no effect on displays of conceptual ("About_") help.`, `SwitchParameter`),
        new Parameter(`Examples`, `Displays only the name, synopsis, and examples. To display only the examples, type "(Get-Help <cmdlet-name>).Examples".

This parameter is effective only when help files are for the command are installed on the computer. It has no effect on displays of conceptual ("About_") help.`, `SwitchParameter`),
        new Parameter(`Full`, `Displays the whole help topic for a cmdlet. This includes parameter descriptions and attributes, examples, input and output object types, and additional notes.

This parameter is effective only when help files are for the command are installed on the computer. It has no effect on displays of conceptual ("About_") help.`, `SwitchParameter`),
        new Parameter(`Functionality`, `Displays help for items with the specified functionality. Enter the functionality. Wildcard characters are permitted.

This parameter has no effect on displays of conceptual ("About_") help.`, `String[]`),
        new Parameter(`Name`, `Gets help about the specified command or concept. Enter the name of a cmdlet, function, provider, script, or workflow, such as "Get-Member", a conceptual topic name, such as "about_Objects", or an alias, such as "ls". Wildcard characters are permitted in cmdlet and provider names, but you cannot use wildcard characters to find the names of function help and script help topics.

To get help for a script that is not located in a path that is listed in the Path environment variable, type the path and file name of the script.

If you enter the exact name of a help topic, Get-Help displays the topic contents. If you enter a word or word pattern that appears in several help topic titles, Get-Help displays a list of the matching titles. If you enter a word that does not match any help topic titles, Get-Help displays a list of topics that include that word in their contents.

The names of conceptual topics, such as "about_Objects", must be entered in English, even in non-English versions of PowerShell.`, `String`),
        new Parameter(`Online`, `Displays the online version of a help topic in the default Internet browser. This parameter is valid only for cmdlet, function, workflow and script help topics. You cannot use the Online parameter in Get-Help commands in a remote session.

For information about supporting this feature in help topics that you write, see about_Comment_Based_Help (http://go.microsoft.com/fwlink/?LinkID=144309), and Supporting Online Help (http://go.microsoft.com/fwlink/?LinkID=242132), and How to Write Cmdlet Help (https://go.microsoft.com/fwlink/?LinkID=123415)in the MSDN library.`, `SwitchParameter`),
        new Parameter(`Parameter`, `Displays only the detailed descriptions of the specified parameters. Wildcards are permitted.

This parameter has no effect on displays of conceptual ("About_") help.`, `String`),
        new Parameter(`Path`, `Gets help that explains how the cmdlet works in the specified provider path. Enter a PowerShell provider path.

This parameter gets a customized version of a cmdlet help topic that explains how the cmdlet works in the specified PowerShell provider path. This parameter is effective only for help about a provider cmdlet and only when the provider includes a custom version of the provider cmdlet help topic in its help file. To use this parameter, install the help file for the module that includes the provider.

To see the custom cmdlet help for a provider path, go to the provider path location and enter a Get-Help command or, from any path location, use the Path parameter of Get-Help to specify the provider path. You can also find custom cmdlet help online in the provider help section of the help topics. For example, you can find help for the New-Item cmdlet in the Wsman:\*\ClientCertificate path (http://go.microsoft.com/fwlink/?LinkID=158676).

For more information about PowerShell providers, see about_Providers (About/about_Providers.md).`, `String`),
        new Parameter(`Role`, `Displays help customized for the specified user role. Enter a role. Wildcard characters are permitted.

Enter the role that the user plays in an organization. Some cmdlets display different text in their help files based on the value of this parameter. This parameter has no effect on help for the core cmdlets.`, `String[]`),
    ], `Displays information about PowerShell commands and concepts.`, `Get-Help [[-Name] <String>] [-Category {Alias | Cmdlet | Provider | General | FAQ | Glossary | HelpFile | ScriptCommand | Function | Filter | ExternalScript | All | DefaultHelp | Workflow | DscResource | Class | Configuration}] [-Component <String[]>] -Detailed [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] [<CommonParameters>]

Get-Help [[-Name] <String>] [-Category {Alias | Cmdlet | Provider | General | FAQ | Glossary | HelpFile | ScriptCommand | Function | Filter | ExternalScript | All | DefaultHelp | Workflow | DscResource | Class | Configuration}] [-Component <String[]>] -Examples [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] [<CommonParameters>]

Get-Help [[-Name] <String>] [-Category {Alias | Cmdlet | Provider | General | FAQ | Glossary | HelpFile | ScriptCommand | Function | Filter | ExternalScript | All | DefaultHelp | Workflow | DscResource | Class | Configuration}] [-Component <String[]>] [-Full] [-Functionality <String[]>] [-Path <String>] [-Role <String[]>] [<CommonParameters>]

Get-Help [[-Name] <String>] [-Category {Alias | Cmdlet | Provider | General | FAQ | Glossary | HelpFile | ScriptCommand | Function | Filter | ExternalScript | All | DefaultHelp | Workflow | DscResource | Class | Configuration}] [-Component <String[]>] [-Functionality <String[]>] -Online [-Path <String>] [-Role <String[]>] [<CommonParameters>]

Get-Help [[-Name] <String>] [-Category {Alias | Cmdlet | Provider | General | FAQ | Glossary | HelpFile | ScriptCommand | Function | Filter | ExternalScript | All | DefaultHelp | Workflow | DscResource | Class | Configuration}] [-Component <String[]>] [-Functionality <String[]>] -Parameter <String> [-Path <String>] [-Role <String[]>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-History`, [
        new Parameter(`Count`, `Specifies the number of the most recent history entries that this cmdlet gets. By, default, Get-History gets all entries in the session history. If you use both the Count and Id parameters in a command, the display ends with the command that is specified by the Id parameter.

In Windows PowerShell 2.0, by default, Get-History gets the 32 most recent entries.`, `Int32`),
        new Parameter(`Id`, `Specifies an array of the IDs of entries in the session history. Get-History gets only specified entries. If you use both the Id and Count parameters in a command, Get-History gets the most recent entries ending with the entry specified by the Id parameter.`, `Int64[]`),
    ], `Gets a list of the commands entered during the current session.`, `Get-History [[-Id] <Int64[]>] [[-Count] <Int32>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Host`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Item`, [
        new Parameter(`Stream`, `This parameter is not supported by any providers installed with PowerShell.`, `String[]`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user-name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies, as a string array, an item or items that this cmdlet excludes in the operation. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.

The Exclude parameter is effective only when the command includes the contents of an item, such as C:\Windows\*, where the wildcard character specifies the contents of the C:\Windows directory.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when this cmdlet gets the objects, rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Indicates that this cmdlet gets items that cannot otherwise be accessed, such as hidden items. Implementation varies from provider to provider. For more information, see about_Providers. Even using the Force parameter, the cmdlet cannot override security restrictions.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies, as a string array, an item or items that this cmdlet includes in the operation. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.

The Include parameter is effective only when the command includes the contents of an item, such as C:\Windows\*, where the wildcard character specifies the contents of the C:\Windows directory.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies a path to the item. Unlike the Path parameter, the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Path`, `Specifies the path to an item. This cmdlet gets the item at the specified location. Wildcards are permitted. This parameter is required, but the parameter name ("Path") is optional.

Use a dot (.) to specify the current location. Use the wildcard character (*) to specify all the items in the current location.`, `String[]`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Gets the item at the specified location.`, `Get-Item [-Stream <String[]>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-UseTransaction] [<CommonParameters>]

Get-Item [-Path] <String[]> [-Stream <String[]>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-ItemProperty`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies, as a string array, an item or items that this cmdlet excludes from the operation. Wildcards are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when this cmdlet gets the objects rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Include`, `Specifies, as a string array, an item or items that this cmdlet includes in the operation.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies a path to the item property. The value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Name`, `Specifies the name of the property or properties to retrieve.`, `String[]`),
        new Parameter(`Path`, `Specifies the path to the item or items.`, `String[]`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Gets the properties of a specified item.`, `Get-ItemProperty [[-Name] <String[]>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Include <String[]>] -LiteralPath <String[]> [-UseTransaction] [<CommonParameters>]

Get-ItemProperty [-Path] <String[]> [[-Name] <String[]>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Include <String[]>] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-ItemPropertyValue`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user. Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you are prompted for a password. This parameter is not supported by any providers that are installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies, as a string array, an item or items that this cmdlet excludes from the operation.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcard characters, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when the cmdlet gets the objects rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Include`, `Specifies, as a string array, an item or items that this cmdlet includes in the operation.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies a path to the item property. In contrast to the Path parameter, the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks.`, `String[]`),
        new Parameter(`Name`, `Specifies the name of the property or properties to retrieve.`, `String[]`),
        new Parameter(`Path`, `Specifies the path to the item or items.`, `String[]`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Gets the value for one or more properties of a specified item.`, `Get-ItemPropertyValue [-Name] <String[]> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Include <String[]>] -LiteralPath <String[]> [-UseTransaction] [<CommonParameters>]

Get-ItemPropertyValue [[-Path] <String[]>] [-Name] <String[]> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Include <String[]>] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Job`, [
        new Parameter(`After`, `Gets completed jobs that ended after the specified date and time. Enter a DateTime object, such as one returned by the Get-Date cmdlet or a string that can be converted to a DateTime object, such as "Dec 1, 2012 2:00 AM" or "11/06".

This parameter works only on custom job types, such as workflow jobs and scheduled jobs, that have an EndTime property. It does not work on standard background jobs, such as those created by using the Start-Job cmdlet. For information about support for this parameter, see the help topic for the job type.

This parameter was introduced in Windows PowerShell 3.0.`, `DateTime`),
        new Parameter(`Before`, `Gets completed jobs that ended before the specified date and time. Enter a DateTime object.

This parameter works only on custom job types, such as workflow jobs and scheduled jobs, that have an EndTime property. It does not work on standard background jobs, such as those created by using the Start-Job cmdlet. For information about support for this parameter, see the help topic for the job type.

This parameter was introduced in Windows PowerShell 3.0.`, `DateTime`),
        new Parameter(`ChildJobState`, `Gets only the child jobs that have the specified state. The acceptable values for this parameter are:

- NotStarted

-  Running

-  Completed

- Failed

- Stopped

- Blocked

- Suspended

- Disconnected

- Suspending

- Stopping



By default, Get-Job does not get child jobs. By using the IncludeChildJob parameter, Get-Job gets all child jobs. If you use the ChildJobState parameter, the IncludeChildJob parameter has no effect.

This parameter was introduced in Windows PowerShell 3.0.`, `JobState`),
        new Parameter(`Command`, `Specifies an array of commands as strings. This cmdlet gets the jobs that include the specified commands. The default is all jobs. You can use wildcard characters to specify a command pattern.`, `String[]`),
        new Parameter(`Filter`, `Specifies a hash table of conditions. This cmdlet gets jobs that satisfy all of the conditions. Enter a hash table where the keys are job properties and the values are job property values.

This parameter works only on custom job types, such as workflow jobs and scheduled jobs. It does not work on standard background jobs, such as those created by using the Start-Job cmdlet. For information about support for this parameter, see the help topic for the job type.

This parameter was introduced in Windows PowerShell 3.0.`, `Hashtable`),
        new Parameter(`HasMoreData`, `Indicates whether this cmdlet gets only jobs that have the specified HasMoreData property value. The HasMoreData property indicates whether all job results have been received in the current session. To get jobs that have more results, specify a value of $True. To get jobs that do not have more results, specify a value of $False.

To get the results of a job, use the Receive-Job cmdlet.

When you use the Receive-Job cmdlet, it deletes from its in-memory, session-specific storage the results that it returned. When it has returned all results of the job in the current session, it sets the value of the HasMoreData property of the job to $False) to indicate that it has no more results for the job in the current session. Use the Keep parameter of Receive-Job to prevent Receive-Job from deleting results and changing the value of the HasMoreData property. For more information, type "Get-Help Receive-Job".

The HasMoreData property is specific to the current session. If results for a custom job type are saved outside of the session, such as the scheduled job type, which saves job results on disk, you can use the Receive-Job cmdlet in a different session to get the job results again, even if the value of HasMoreData is $False. For more information, see the help topics for the custom job type.

This parameter was introduced in Windows PowerShell 3.0.`, `Boolean`),
        new Parameter(`Id`, `Specifies an array of IDs of jobs that this cmdlet gets.

The ID is an integer that uniquely identifies the job in the current session. It is easier to remember and to type than the instance ID, but it is unique only in the current session. You can type one or more IDs separated by commas. To find the ID of a job, type "Get-Job" without parameters.`, `Int32[]`),
        new Parameter(`IncludeChildJob`, `Indicates that this cmdlet returns child jobs, in addition to parent jobs.

This parameter is especially useful for investigating workflow jobs, for which Get-Job returns a container parent job, and job failures, because the reason for the failure is saved in a property of the child job.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`InstanceId`, `Specifies an array of instance IDs of jobs that this cmdlet gets. The default is all jobs.

An instance ID is a GUID that uniquely identifies the job on the computer. To find the instance ID of a job, use Get-Job .`, `Guid[]`),
        new Parameter(`Name`, `Specifies an array of instance friendly names of jobs that this cmdlet gets. Enter a job name, or use wildcard characters to enter a job name pattern. By default, Get-Job gets all jobs in the current session.`, `String[]`),
        new Parameter(`Newest`, `Specifies a number of jobs to get. This cmdlet gets the jobs that ended most recently.

The Newest parameter does not sort or return the newest jobs in end-time order. To sort the output, use the Sort-Object cmdlet.

This parameter was introduced in Windows PowerShell 3.0.`, `Int32`),
        new Parameter(`State`, `Specifies a job state. This cmdlet gets only jobs in the specified state. The acceptable values for this parameter are:

- NotStarted

- Running

- Completed

- Failed

- Stopped

- Blocked

- Suspended

- Disconnected

- Suspending

- Stopping



By default, Get-Job gets all the jobs in the current session.

For more information about job states, see JobState Enumeration (https://msdn.microsoft.com/library/system.management.automation.jobstate)in the MSDN library.`, `JobState`),
    ], `Gets PowerShell background jobs that are running in the current session.`, `Get-Job [[-Id] <Int32[]>] [-After <DateTime>] [-Before <DateTime>] [-ChildJobState {NotStarted | Running | Completed | Failed | Stopped | Blocked | Suspended | Disconnected | Suspending | Stopping | AtBreakpoint}] [-HasMoreData <Boolean>] [-IncludeChildJob] [-Newest <Int32>] [<CommonParameters>]

Get-Job [-InstanceId] <Guid[]> [-After <DateTime>] [-Before <DateTime>] [-ChildJobState {NotStarted | Running | Completed | Failed | Stopped | Blocked | Suspended | Disconnected | Suspending | Stopping | AtBreakpoint}] [-HasMoreData <Boolean>] [-IncludeChildJob] [-Newest <Int32>] [<CommonParameters>]

Get-Job [-Name] <String[]> [-After <DateTime>] [-Before <DateTime>] [-ChildJobState {NotStarted | Running | Completed | Failed | Stopped | Blocked | Suspended | Disconnected | Suspending | Stopping | AtBreakpoint}] [-HasMoreData <Boolean>] [-IncludeChildJob] [-Newest <Int32>] [<CommonParameters>]

Get-Job [-State] {NotStarted | Running | Completed | Failed | Stopped | Blocked | Suspended | Disconnected | Suspending | Stopping | AtBreakpoint} [-After <DateTime>] [-Before <DateTime>] [-ChildJobState {NotStarted | Running | Completed | Failed | Stopped | Blocked | Suspended | Disconnected | Suspending | Stopping | AtBreakpoint}] [-HasMoreData <Boolean>] [-IncludeChildJob] [-Newest <Int32>] [<CommonParameters>]

Get-Job [-After <DateTime>] [-Before <DateTime>] [-ChildJobState {NotStarted | Running | Completed | Failed | Stopped | Blocked | Suspended | Disconnected | Suspending | Stopping | AtBreakpoint}] [-Command <String[]>] [-HasMoreData <Boolean>] [-IncludeChildJob] [-Newest <Int32>] [<CommonParameters>]

Get-Job [-Filter] <Hashtable> [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Location`, [
        new Parameter(`PSDrive`, `Specifies the current location in the specified PowerShell drive that this cmdlet gets in the operation.

For instance, if you are in the Certificate: drive, you can use this parameter to find your current location in the C: drive.`, `String[]`),
        new Parameter(`PSProvider`, `Specifies the current location in the drive supported by the PowerShell provider that this cmdlet gets in the operation.

If the specified provider supports more than one drive, this cmdlet returns the location on the most recently accessed drive.

For example, if you are in the C: drive, you can use this parameter to find your current location in the drives of the PowerShellRegistry provider.`, `String[]`),
        new Parameter(`Stack`, `Indicates that this cmdlet displays the locations in the current location stack.

To display the locations in a different location stack, use the StackName parameter. For information about location stacks, see the Notes.`, `SwitchParameter`),
        new Parameter(`StackName`, `Specifies, as a string array, the named location stacks. Enter one or more location stack names.

To display the locations in the current location stack, use the Stack parameter. To make a location stack the current location stack, use the Set-Location parameter. For information about location stacks, see the Notes.

This cmdlet cannot display the locations in the unnamed default stack unless it is the current stack.`, `String[]`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Gets information about the current working location or a location stack.`, `Get-Location [-PSDrive <String[]>] [-PSProvider <String[]>] [-UseTransaction] [<CommonParameters>]

Get-Location [-Stack] [-StackName <String[]>] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Member`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Module`, [
        new Parameter(`All`, `Indicates that this cmdlet gets all modules in each module folder, including nested modules, manifest (.psd1) files, script module (.psm1) files, and binary module (.dll) files. Without this parameter, "Get-Module" gets only the default module in each module folder.`, `SwitchParameter`),
        new Parameter(`CimNamespace`, `Specifies the namespace of an alternate CIM provider that exposes CIM modules. The default value is the namespace of the Module Discovery WMI provider.

Use this parameter to get CIM modules from computers and devices that are not running the Windows operating system.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`CimResourceUri`, `Specifies an alternate location for CIM modules. The default value is the resource URI of the Module Discovery WMI provider on the remote computer.

Use this parameter to get CIM modules from computers and devices that are not running the Windows operating system.

This parameter was introduced in Windows PowerShell 3.0.`, `Uri`),
        new Parameter(`CimSession`, `Specifies a CIM session on the remote computer. Enter a variable that contains the CIM session or a command that gets the CIM session, such as a Get-CimSession (https://docs.microsoft.com/en-us/powershell/module/cimcmdlets/get-cimsession)command.

"Get-Module" uses the CIM session connection to get modules from the remote computer. When you import the module by using the "Import-Module" cmdlet and use the commands from the imported module in the current session, the commands actually run on the remote computer.

You can use this parameter to get modules from computers and devices that are not running the Windows operating system, and computers that have PowerShell, but do not have PowerShell remoting enabled.

The CimSession parameter gets all modules in the CIMSession . However, you can import only CIM-based and Cmdlet Definition XML (CDXML)-based modules.`, `CimSession`),
        new Parameter(`FullyQualifiedName`, `Specifies names of modules in the form of ModuleSpecification objects. These objects are described in the Remarks section of ModuleSpecification Constructor (Hashtable) (https://msdn.microsoft.com/library/jj136290)in the MSDN library. For example, the FullyQualifiedName parameter accepts a module name that is specified in the following formats:

- @{ModuleName = "modulename"; ModuleVersion = "version_number"}

- @{ModuleName = "modulename"; ModuleVersion = "version_number"; Guid = "GUID"} ModuleName and ModuleVersion are required, but Guid is optional.

You cannot specify the FullyQualifiedName parameter in the same command as a Name parameter.`, `ModuleSpecification[]`),
        new Parameter(`ListAvailable`, `Indicates that this cmdlet gets all installed modules. "Get-Module" gets modules in paths listed in the PSModulePath environment variable. Without this parameter, "Get-Module" gets only the modules that are both listed in the PSModulePath environment variable, and that are loaded in the current session. ListAvailable does not return information about modules that are not found in the PSModulePath environment variable, even if those modules are loaded in the current session.`, `SwitchParameter`),
        new Parameter(`Name`, `Specifies names or name patterns of modules that this cmdlet gets. Wildcard characters are permitted. You can also pipe the names to "Get-Module". You cannot specify the FullyQualifiedName parameter in the same command as a Name parameter. Name cannot accept a module GUID as a value. To return modules by specifying a GUID, use FullyQualifiedName instead.`, `String[]`),
        new Parameter(`PSSession`, `Gets the modules in the specified user-managed PowerShell session ( PSSession ). Enter a variable that contains the session, a command that gets the session, such as a "Get-PSSession" command, or a command that creates the session, such as a "New-PSSession" command.

When the session is connected to a remote computer, you must specify the ListAvailable parameter.

A "Get-Module" command that uses the PSSession parameter is equivalent to using the "Invoke-Command" cmdlet to run a "Get-Module -ListAvailable" command in a PSSession .

This parameter was introduced in Windows PowerShell 3.0.`, `PSSession`),
        new Parameter(`Refresh`, `Indicates that this cmdlet refreshes the cache of installed commands. The command cache is created when the session starts. It enables the "Get-Command" cmdlet to get commands from modules that are not imported into the session.

This parameter is designed for development and testing scenarios in which the contents of modules have changed since the session started.

When you specify the Refresh parameter in a command, you must specify ListAvailable .

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`PSEdition`, `Gets the modules that support specified edition of PowerShell.

The acceptable values for this parameter are:

- Desktop

- Core



The Get-Module cmdlet checks CompatiblePSEditions property of PSModuleInfo object for the specified value and returns only those modules that have it set.

> [!NOTE] > - Desktop Edition: Built on .NET Framework and provides compatibility with scripts and modules targeting versions of PowerShell running on full footprint editions of Windows such as Server Core and Windows Desktop. > - Core Edition: Built on .NET Core and provides compatibility with scripts and modules targeting versions of PowerShell running on reduced footprint editions of Windows such as Nano Server and Windows IoT.`, `String`),
    ], `Gets the modules that have been imported or that can be imported into the current session.`, `Get-Module [[-Name] <String[]>] [-All] [-FullyQualifiedName <ModuleSpecification[]>] [<CommonParameters>]

Get-Module [[-Name] <String[]>] [-All] [-FullyQualifiedName <ModuleSpecification[]>] -ListAvailable [-Refresh] [-PSEdition <String>] [<CommonParameters>]

Get-Module [[-Name] <String[]>] [-CimNamespace <String>] [-CimResourceUri <Uri>] -CimSession <CimSession> [-FullyQualifiedName <ModuleSpecification[]>] [-ListAvailable] [-Refresh] [<CommonParameters>]

Get-Module [[-Name] <String[]>] [-FullyQualifiedName <ModuleSpecification[]>] [-ListAvailable] -PSSession <PSSession> [-Refresh] [-PSEdition <String>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Package`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PackageProvider`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PackageSource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PfxCertificate`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Process`, [
        new Parameter(`FileVersionInfo`, `Indicates that this cmdlet gets the file version information for the program that runs in the process.

On Windows Vista and later versions of Windows, you must open PowerShell with the Run as administrator option to use this parameter on processes that you do not own.

To get file version information for a process on a remote computer, use the Invoke-Command cmdlet.

Using this parameter is equivalent to getting the MainModule.FileVersionInfo property of each process object. When you use this parameter, Get-Process returns a FileVersionInfo object (System.Diagnostics.FileVersionInfo), not a process object. So, you cannot pipe the output of the command to a cmdlet that expects a process object, such as Stop-Process.`, `SwitchParameter`),
        new Parameter(`Id`, `Specifies one or more processes by process ID (PID). To specify multiple IDs, use commas to separate the IDs. To find the PID of a process, type "Get-Process".`, `Int32[]`),
        new Parameter(`IncludeUserName`, `Indicates that the UserName value of the Process object is returned with results of the command.`, `SwitchParameter`),
        new Parameter(`InputObject`, `Specifies one or more process objects. Enter a variable that contains the objects, or type a command or expression that gets the objects.`, `Process[]`),
        new Parameter(`Module`, `Indicates that this cmdlet gets the modules that have been loaded by the processes.

On Windows Vista and later versions of Windows, you must open PowerShell with the Run as administrator option to use this parameter on processes that you do not own.

To get the modules that have been loaded by a process on a remote computer, use the Invoke-Command cmdlet.

This parameter is equivalent to getting the Modules property of each process object. When you use this parameter, this cmdlet returns a ProcessModule object (System.Diagnostics.ProcessModule), not a process object. So, you cannot pipe the output of the command to a cmdlet that expects a process object, such as Stop-Process.

When you use both the Module and FileVersionInfo parameters in the same command, this cmdlet returns a FileVersionInfo object with information about the file version of all modules.`, `SwitchParameter`),
        new Parameter(`Name`, `Specifies one or more processes by process name. You can type multiple process names (separated by commas) and use wildcard characters. The parameter name ("Name") is optional.`, `String[]`),
    ], `Gets the processes that are running on the local computer.`, `Get-Process [[-Name] <String[]>] [-FileVersionInfo] [-Module] [<CommonParameters>]

Get-Process [-FileVersionInfo] -Id <Int32[]> [-Module] [<CommonParameters>]

Get-Process [-FileVersionInfo] -InputObject <Process[]> [-Module] [<CommonParameters>]

Get-Process -Id <Int32[]> -IncludeUserName [<CommonParameters>]

Get-Process [[-Name] <String[]>] -IncludeUserName [<CommonParameters>]

Get-Process -IncludeUserName -InputObject <Process[]> [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSBreakpoint`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSCallStack`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSDrive`, [
        new Parameter(`LiteralName`, `Specifies the name of the drive.

The value of LiteralName is used exactly as it is typed. No characters are interpreted as wildcards. If the name includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Name`, `Specifies, as a string array, the name or name of drives that this cmdlet gets in the operation. Type the drive name or letter without a colon (:).`, `String[]`),
        new Parameter(`PSProvider`, `Specifies, as a string array, the PowerShell provider. This cmdlet gets only the drives supported by this provider. Type the name of a provider, such as FileSystem, Registry, or Certificate.`, `String[]`),
        new Parameter(`Scope`, `Specifies the scope in which this cmdlet gets the drives.

The acceptable values for this parameter are:

- Global

- Local

- Script

- a number relative to the current scope (0 through the number of scopes, where 0 is the current scope and 1 is its parent).

"Local" is the default. For more information, see about_Scopes (http://go.microsoft.com/fwlink/?LinkID=113260).`, `String`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Gets drives in the current session.`, `Get-PSDrive [-LiteralName] <String[]> [-PSProvider <String[]>] [-Scope <String>] [-UseTransaction] [<CommonParameters>]

Get-PSDrive [[-Name] <String[]>] [-PSProvider <String[]>] [-Scope <String>] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSHostProcessInfo`, [
        new Parameter(`Id`, ``, `Int32[]`),
        new Parameter(`Name`, ``, `String[]`),
        new Parameter(`Process`, ``, `Process[]`),
    ], ``, `Get-PSHostProcessInfo [-Id] <Int32[]> [<CommonParameters>]

Get-PSHostProcessInfo [[-Name] <String[]>] [<CommonParameters>]

Get-PSHostProcessInfo [-Process] <Process[]> [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSProvider`, [
        new Parameter(`PSProvider`, `Specifies the name or names of the PowerShell providers about which this cmdlet gets information.`, `String[]`),
    ], `Gets information about the specified PowerShell provider.`, `Get-PSProvider [[-PSProvider] <String[]>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSReadlineKeyHandler`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSReadlineOption`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSSession`, [
        new Parameter(`AllowRedirection`, `Indicates that this cmdlet allows redirection of this connection to an alternate Uniform Resource Identifier (URI). By default, PowerShell does not redirect connections.

This parameter configures the temporary connection that is created to run a Get-PSSession command with the ConnectionUri parameter.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`ApplicationName`, `Specifies the name of an application. This cmdlet connects only to sessions that use the specified application.

Enter the application name segment of the connection URI. For example, in the following connection URI, the application name is WSMan: "http://localhost:5985/WSMAN". The application name of a session is stored in the Runspace.ConnectionInfo.AppName property of the session.

The value of this parameter is used to select and filter sessions. It does not change the application that the session uses.`, `String`),
        new Parameter(`Authentication`, `Specifies the mechanism that is used to authenticate credentials for the session in which the Get-PSSession command runs.

This parameter configures the temporary connection that is created to run a Get-PSSession command with the ComputerName or ConnectionUri parameter.

The acceptable values for this parameter are:

- Default

- Basic

- Credssp

- Digest

- Kerberos

- Negotiate

- NegotiateWithImplicitCredential.



The default value is Default.

For more information about the values of this parameter, see AuthenticationMechanism Enumeration (https://msdn.microsoft.com/library/system.management.automation.runspaces.authenticationmechanism)in the MSDN library.

CAUTION: Credential Security Support Provider (CredSSP) authentication, in which the user's credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.

This parameter was introduced in Windows PowerShell 3.0.`, `AuthenticationMechanism`),
        new Parameter(`CertificateThumbprint`, `Specifies the digital public key certificate (X509) of a user account that has permission to create the session in which the Get-PSSession command runs. Enter the certificate thumbprint of the certificate.

This parameter configures the temporary connection that is created to run a Get-PSSession command with the ComputerName or ConnectionUri parameter.

Certificates are used in client certificate-based authentication. They can be mapped only to local user accounts; they do not work with domain accounts.

To get a certificate thumbprint, use a Get-Item or Get-ChildItem command in the PowerShell Cert: drive.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`ComputerName`, `Specifies an array of names of computers. Gets the sessions that connect to the specified computers. Wildcard characters are not permitted. There is no default value.

Beginning in Windows PowerShell 3.0, PSSession objects are stored on the computers at the remote end of each connection. To get the sessions on the specified computers, PowerShell creates a temporary connection to each computer and runs a Get-PSSession command.

Type the NetBIOS name, an IP address, or a fully-qualified domain name of one or more computers. To specify the local computer, type the computer name, localhost, or a dot (.).

Note: This parameter gets sessions only from computers that run Windows PowerShell 3.0 or later versions of PowerShell. Earlier versions do not store sessions.`, `String[]`),
        new Parameter(`ConfigurationName`, `Specifies the name of a configuration. This cmdlet gets only to sessions that use the specified session configuration.

Enter a configuration name or the fully qualified resource URI for a session configuration. If you specify only the configuration name, the following schema URI is prepended: http://schemas.microsoft.com/powershell. The configuration name of a session is stored in the ConfigurationName property of the session.

The value of this parameter is used to select and filter sessions. It does not change the session configuration that the session uses.

For more information about session configurations, see about_Session_Configurations (About/about_Session_Configurations.md).`, `String`),
        new Parameter(`ConnectionUri`, `Specifies a URI that defines the connection endpoint for the temporary session in which the Get-PSSession command runs. The URI must be fully qualified.

This parameter configures the temporary connection that is created to run a Get-PSSession command with the ConnectionUri parameter.

The format of this string is:

<Transport>://<ComputerName>:<Port>/<ApplicationName>

The default value is: http://localhost:5985/WSMAN.

If you do not specify a ConnectionUri , you can use the UseSSL , ComputerName , Port , and ApplicationName parameters to specify the ConnectionURI values. Valid values for the Transport segment of the URI are HTTP and HTTPS. If you specify a connection URI with a Transport segment, but do not specify a port, the session is created with standards ports: 80 for HTTP and 443 for HTTPS. To use the default ports for PowerShell remoting, specify port 5985 for HTTP or 5986 for HTTPS.

If the destination computer redirects the connection to a different URI, PowerShell prevents the redirection unless you use the AllowRedirection parameter in the command.

This parameter was introduced in Windows PowerShell 3.0.

This parameter gets sessions only from computers that run Windows PowerShell 3.0 or later versions of Windows PowerShell or PowerShell Core. Earlier versions do not store sessions.`, `Uri[]`),
        new Parameter(`Credential`, `Specifies a user credential. This cmdlet runs the command with the permissions of the specified user. Specify a user account that has permission to connect to the remote computer and run a Get-PSSession command. The default is the current user. Type a user name, such as "User01", "Domain01\User01", or "User@Domain.com", or enter a PSCredential object, such as one returned by the Get-Credential cmdlet. When you type a user name, this cmdlet prompts you for a password.

This parameter configures to the temporary connection that is created to run a Get-PSSession command with the ComputerName or ConnectionUri parameter.

This parameter was introduced in Windows PowerShell 3.0.`, `PSCredential`),
        new Parameter(`Id`, `Specifies an array of session IDs. This cmdlet gets only the sessions with the specified IDs. Type one or more IDs, separated by commas, or use the range operator (..) to specify a range of IDs. You cannot use the ID parameter together with the ComputerName parameter.

An ID is an integer that uniquely identifies the user-managed sessions in the current session. It is easier to remember and type than the InstanceId , but it is unique only within the current session. The ID of a session is stored in the ID property of the session.`, `Int32[]`),
        new Parameter(`InstanceId`, `Specifies an array of instance IDs of sessions. This cmdlet gets only the sessions with the specified instance IDs.

The instance ID is a GUID that uniquely identifies a session on a local or remote computer. The InstanceID is unique, even when you have multiple sessions running in PowerShell.

The instance ID of a session is stored in the InstanceID property of the session.`, `Guid[]`),
        new Parameter(`Name`, `Specifies an array of session names. This cmdlet gets only the sessions that have the specified friendly names. Wildcard characters are permitted.

The friendly name of a session is stored in the Name property of the session.`, `String[]`),
        new Parameter(`Port`, `Specifies the specified network port that is used for the temporary connection in which the Get-PSSession command runs. To connect to a remote computer, the remote computer must be listening on the port that the connection uses. The default ports are 5985, which is the WinRM port for HTTP, and 5986, which is the WinRM port for HTTPS.

Before using an alternate port, you must configure the WinRM listener on the remote computer to listen at that port. To configure the listener, type the following two commands at the PowerShell prompt:

"Remove-Item -Path WSMan:\Localhost\listener\listener* -Recurse"

"New-Item -Path WSMan:\Localhost\listener -Transport http -Address * -Port <port-number>"

This parameter configures to the temporary connection that is created to run a Get-PSSession command with the ComputerName or ConnectionUri parameter.

Do not use the Port parameter unless you must. The Port set in the command applies to all computers or sessions on which the command runs. An alternate port setting might prevent the command from running on all computers.

This parameter was introduced in Windows PowerShell 3.0.`, `Int32`),
        new Parameter(`SessionOption`, `Specifies advanced options for the session. Enter a SessionOption object, such as one that you create by using the New-PSSessionOption cmdlet, or a hash table in which the keys are session option names and the values are session option values.

The default values for the options are determined by the value of the $PSSessionOption preference variable, if it is set. Otherwise, the default values are established by options set in the session configuration.

The session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration.

For a description of the session options, including the default values, see New-PSSessionOption. For information about the $PSSessionOption preference variable, see about_Preference_Variables (About/about_Preference_Variables.md)in the Microsoft TechNet library. For more information about session configurations, see about_Session_Configurations (About/about_Session_Configurations.md).`, `PSSessionOption`),
        new Parameter(`State`, `Specifies a session state. This cmdlet gets only sessions in the specified state. The acceptable values for this parameter are: All, Opened, Disconnected, Closed, and Broken. The default value is All.

The session state value is relative to the current sessions. Sessions that were not created in the current sessions and are not connected to the current session have a state of Disconnected even when they are connected to a different session.

The state of a session is stored in the State property of the session.

This parameter was introduced in Windows PowerShell 3.0.`, `SessionFilterState`),
        new Parameter(`ThrottleLimit`, `Specifies the maximum number of concurrent connections that can be established to run the Get-PSSession command. If you omit this parameter or enter a value of 0 (zero), the default value, 32, is used. The throttle limit applies only to the current command, not to the session or to the computer.

This parameter was introduced in Windows PowerShell 3.0.`, `Int32`),
        new Parameter(`UseSSL`, `Indicates that this cmdlet uses the Secure Sockets Layer (SSL) protocol to establish the connection in which the Get-PSSession command runs. By default, SSL is not used. If you use this parameter, but SSL is not available on the port used for the command, the command fails.

This parameter configures the temporary connection that is created to run a Get-PSSession command with the ComputerName parameter.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`ContainerId`, `Specifies an array of IDs of containers. This cmdlet starts an interactive session with each of the specified containers. To see the containers that are available to you, use the Get-Container cmdlet.`, `String[]`),
        new Parameter(`VMId`, `Specifies an array of ID of virtual machines. This cmdlet starts an interactive session with each of the specified virtual machines. To see the virtual machines that are available to you, use the following command:

"Get-VM | Select-Object -Property Name, ID"`, `Guid[]`),
        new Parameter(`VMName`, `Specifies an array of names of virtual machines. This cmdlet starts an interactive session with each of the specified virtual machines. To see the virtual machines that are available to you, use the Get-VM cmdlet.`, `String[]`),
    ], `Gets the PowerShell sessions on local and remote computers.`, `Get-PSSession [-ConnectionUri] <Uri[]> [-AllowRedirection] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Credential <PSCredential>] -InstanceId <Guid[]> [-SessionOption <PSSessionOption>] [-State {All | Opened | Disconnected | Closed | Broken}] [-ThrottleLimit <Int32>] [<CommonParameters>]

Get-PSSession [-ConnectionUri] <Uri[]> [-AllowRedirection] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Credential <PSCredential>] [-Name <String[]>] [-SessionOption <PSSessionOption>] [-State {All | Opened | Disconnected | Closed | Broken}] [-ThrottleLimit <Int32>] [<CommonParameters>]

Get-PSSession [-ComputerName] <String[]> [-ApplicationName <String>] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Credential <PSCredential>] [-Name <String[]>] [-Port <Int32>] [-SessionOption <PSSessionOption>] [-State {All | Opened | Disconnected | Closed | Broken}] [-ThrottleLimit <Int32>] [-UseSSL] [<CommonParameters>]

Get-PSSession [-ComputerName] <String[]> [-ApplicationName <String>] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Credential <PSCredential>] -InstanceId <Guid[]> [-Port <Int32>] [-SessionOption <PSSessionOption>] [-State {All | Opened | Disconnected | Closed | Broken}] [-ThrottleLimit <Int32>] [-UseSSL] [<CommonParameters>]

Get-PSSession [-ConfigurationName <String>] -InstanceId <Guid[]> [-State {All | Opened | Disconnected | Closed | Broken}] -VMId <Guid[]> [<CommonParameters>]

Get-PSSession [-ConfigurationName <String>] [-Name <String[]>] [-State {All | Opened | Disconnected | Closed | Broken}] -ContainerId <String[]> [<CommonParameters>]

Get-PSSession [-ConfigurationName <String>] -InstanceId <Guid[]> [-State {All | Opened | Disconnected | Closed | Broken}] -ContainerId <String[]> [<CommonParameters>]

Get-PSSession [-ConfigurationName <String>] [-Name <String[]>] [-State {All | Opened | Disconnected | Closed | Broken}] -VMId <Guid[]> [<CommonParameters>]

Get-PSSession [-ConfigurationName <String>] [-Name <String[]>] [-State {All | Opened | Disconnected | Closed | Broken}] -VMName <String[]> [<CommonParameters>]

Get-PSSession [-ConfigurationName <String>] -InstanceId <Guid[]> [-State {All | Opened | Disconnected | Closed | Broken}] -VMName <String[]> [<CommonParameters>]

Get-PSSession [-Id] <Int32[]> [<CommonParameters>]

Get-PSSession [-InstanceId <Guid[]>] [<CommonParameters>]

Get-PSSession [-Name <String[]>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSSessionCapability`, [
        new Parameter(`ConfigurationName`, `Specifies the constrained session configuration (endpoint) that you are inspecting.`, `String`),
        new Parameter(`Full`, `Indicates that this cmdlet returns the entire initial session state for the specified user at the specified constrained endpoint.`, `SwitchParameter`),
        new Parameter(`Username`, `Specifies the user whose capabilities you are inspecting.`, `String`),
    ], `Gets the capabilities of a specific user on a constrained session configuration.`, `Get-PSSessionCapability [-ConfigurationName] <String> [-Username] <String> [-Full] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-PSSessionConfiguration`, [
        new Parameter(`Force`, `Suppresses the prompt to restart the WinRM service, if the service is not already running.`, `SwitchParameter`),
        new Parameter(`Name`, `Specifies an array of names. This cmdlet gets the session configurations with the specified name or name pattern. Enter one or more session configuration names. Wildcard characters are permitted.`, `String[]`),
    ], `Gets the registered session configurations on the computer.`, `Get-PSSessionConfiguration [[-Name] <String[]>] [-Force] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Random`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Runspace`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-RunspaceDebug`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Service`, [
        new Parameter(`DependentServices`, `Indicates that this cmdlet gets only the services that depend upon the specified service.

By default, this cmdlet gets all services.`, `SwitchParameter`),
        new Parameter(`DisplayName`, `Specifies, as a string array, the display names of services to be retrieved. Wildcards are permitted. By default, this cmdlet gets all services on the computer.`, `String[]`),
        new Parameter(`Exclude`, `Specifies, as a string array, a service or services that this cmdlet excludes from the operation. The value of this parameter qualifies the Name parameter. Enter a name element or pattern, such as "s*". Wildcards are permitted.`, `String[]`),
        new Parameter(`Include`, `Specifies, as a string array, a service or services that this cmdlet includes in the operation. The value of this parameter qualifies the Name parameter. Enter a name element or pattern, such as "s*". Wildcards are permitted.`, `String[]`),
        new Parameter(`InputObject`, `Specifies ServiceController objects representing the services to be retrieved. Enter a variable that contains the objects, or type a command or expression that gets the objects. You can also pipe a service object to this cmdlet.`, `ServiceController[]`),
        new Parameter(`Name`, `Specifies the service names of services to be retrieved. Wildcards are permitted. By default, this cmdlet gets all of the services on the computer.`, `String[]`),
        new Parameter(`RequiredServices`, `Indicates that this cmdlet gets only the services that this service requires.

This parameter gets the value of the ServicesDependedOn property of the service. By default, this cmdlet gets all services.`, `SwitchParameter`),
    ], `Gets the services on the computer.`, `Get-Service [-DependentServices] -DisplayName <String[]> [-Exclude <String[]>] [-Include <String[]>] [-RequiredServices] [<CommonParameters>]

Get-Service [-DependentServices] [-Exclude <String[]>] [-Include <String[]>] [-InputObject <ServiceController[]>] [-RequiredServices] [<CommonParameters>]

Get-Service [[-Name] <String[]>] [-DependentServices] [-Exclude <String[]>] [-Include <String[]>] [-RequiredServices] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-TimeZone`, [
        new Parameter(`Id`, `Specifies, as a string array, the ID or IDs of the time zones that this cmdlet gets.`, `String[]`),
        new Parameter(`ListAvailable`, `Indicates that this cmdlet gets all available time zones.`, `SwitchParameter`),
        new Parameter(`Name`, `Specifies, as a string array, the name or names of the time zones that this cmdlet gets.`, `String[]`),
    ], `Gets the current time zone or a list of available time zones.`, `Get-TimeZone -Id <String[]> [<CommonParameters>]

Get-TimeZone -ListAvailable [<CommonParameters>]

Get-TimeZone [[-Name] <String[]>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-TraceSource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-TypeData`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-UICulture`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Unique`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Uptime`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Variable`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-Verb`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-WinEvent`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-WSManCredSSP`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Get-WSManInstance`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Group-Object`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Import-Alias`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Import-BinaryMiLog`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Import-Clixml`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Import-Csv`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Import-LocalizedData`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Import-Module`, [
        new Parameter(`Alias`, `Specifies the aliases that this cmdlet imports from the module into the current session. Enter a comma-separated list of aliases. Wildcard characters are permitted.

Some modules automatically export selected aliases into your session when you import the module. This parameter lets you select from among the exported aliases.`, `String[]`),
        new Parameter(`ArgumentList`, `Specifies an array of arguments, or parameter values, that are passed to a script module during the Import-Module command. This parameter is valid only when you are importing a script module.

You can also refer to the ArgumentList parameter by its alias, args . For more information, see about_Aliases.`, `Object[]`),
        new Parameter(`AsCustomObject`, `Indicates that this cmdlet returns a custom object with members that represent the imported module members. This parameter is valid only for script modules.

When you use the AsCustomObject parameter, Import-Module imports the module members into the session and then returns a PSCustomObject object instead of a PSModuleInfo object. You can save the custom object in a variable and use dot notation to invoke the members.`, `SwitchParameter`),
        new Parameter(`Assembly`, `Specifies an array of assembly objects. This cmdlet imports the cmdlets and providers implemented in the specified assembly objects. Enter a variable that contains assembly objects or a command that creates assembly objects. You can also pipe an assembly object to Import-Module .

When you use this parameter, only the cmdlets and providers implemented by the specified assemblies are imported. If the module contains other files, they are not imported, and you might be missing important members of the module. Use this parameter for debugging and testing the module, or when you are instructed to use it by the module author.`, `Assembly[]`),
        new Parameter(`CimNamespace`, `Specifies the namespace of an alternate CIM provider that exposes CIM modules. The default value is the namespace of the Module Discovery WMI provider.

Use this parameter to import CIM modules from computers and devices that are not running a Windows operating system.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`CimResourceUri`, `Specifies an alternate location for CIM modules. The default value is the resource URI of the Module Discovery WMI provider on the remote computer.

Use this parameter to import CIM modules from computers and devices that are not running a Windows operating system.

This parameter was introduced in Windows PowerShell 3.0.`, `Uri`),
        new Parameter(`CimSession`, `Specifies a CIM session on the remote computer. Enter a variable that contains the CIM session or a command that gets the CIM session, such as a Get-CimSession (../CimCmdlets/Get-CimSession.md)command. Import-Module uses the CIM session connection to import modules from the remote computer into the current session. When you use the commands from the imported module in the current session, the commands actually run on the remote computer.

You can use this parameter to import modules from computers and devices that are not running the Windows operating system, and Windows computers that have PowerShell, but do not have PowerShell remoting enabled.

This parameter was introduced in Windows PowerShell 3.0.`, `CimSession`),
        new Parameter(`Cmdlet`, `Specifies an array of cmdlets that this cmdlet imports from the module into the current session. Wildcard characters are permitted.

Some modules automatically export selected cmdlets into your session when you import the module. This parameter lets you select from among the exported cmdlets.`, `String[]`),
        new Parameter(`DisableNameChecking`, `Indicates that this cmdlet suppresses the message that warns you when you import a cmdlet or function whose name includes an unapproved verb or a prohibited character.

By default, when a module that you import exports cmdlets or functions that have unapproved verbs in their names, PowerShell displays the following warning message:

"WARNING: Some imported command names include unapproved verbs which might make them less discoverable. Use the Verbose parameter for more detail or type Get-Verb to see the list of approved verbs."

This message is only a warning. The complete module is still imported, including the non-conforming commands. Although the message is displayed to module users, the naming problem should be fixed by the module author.`, `SwitchParameter`),
        new Parameter(`Force`, `This parameter causes a module to be loaded, or reloaded, over top of the current one`, `SwitchParameter`),
        new Parameter(`FullyQualifiedName`, `Specifies the fully qualified name of the module specification.`, `ModuleSpecification[]`),
        new Parameter(`Function`, `Specifies an array of functions that this cmdlet imports from the module into the current session. Wildcard characters are permitted.

Some modules automatically export selected functions into your session when you import the module. This parameter lets you select from among the exported functions.`, `String[]`),
        new Parameter(`Global`, `Indicates that this cmdlet imports modules into the global session state so they are available to all commands in the session.

By default, when Import-Module cmdlet is called from the command prompt, script file, or scriptblock, all the commands are imported into the global session state.

When invoked from another module, Import-Module cmdlet imports the commands in a module, including commands from nested modules, into the caller's session state.

The Global parameter is equivalent to the Scope parameter with a value of Global.

To restrict the commands that a module exports, use an "Export-ModuleMember" command in the script module.`, `SwitchParameter`),
        new Parameter(`MinimumVersion`, `Specifies a minimum version. This cmdlet imports only a version of the module that is greater than or equal to the specified value. If no version qualifies, Import-Module generates an error.

By default, Import-Module imports the module without checking the version number.

Use the MinimumVersion parameter name or its alias, Version.

To specify an exact version, use the RequiredVersion parameter. You can also use the Module and Version parameters of the #Requires keyword to require a specific version of a module in a script.

This parameter was introduced in Windows PowerShell 3.0.`, `Version`),
        new Parameter(`ModuleInfo`, `Specifies an array of module objects to import. Enter a variable that contains the module objects, or a command that gets the module objects, such as the following command: "Get-Module -ListAvailable". You can also pipe module objects to Import-Module .`, `PSModuleInfo[]`),
        new Parameter(`Name`, `Specifies the names of the modules to import. Enter the name of the module or the name of a file in the module, such as a .psd1, .psm1, .dll, or ps1 file. File paths are optional. Wildcard characters are not permitted. You can also pipe module names and file names to Import-Module .

If you omit a path, Import-Module looks for the module in the paths saved in the PSModulePath environment variable ($env:PSModulePath).

Specify only the module name whenever possible. When you specify a file name, only the members that are implemented in that file are imported. If the module contains other files, they are not imported, and you might be missing important members of the module.`, `String[]`),
        new Parameter(`NoClobber`, `Indicates that this cmdlet does not import commands that have the same names as existing commands in the current session. By default, Import-Module imports all exported module commands.

Commands that have the same names can hide or replace commands in the session. To avoid command name conflicts in a session, use the Prefix or NoClobber parameters. For more information about name conflicts and command precedence, see "Modules and Name Conflicts" in about_Modules and about_Command_Precedence.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`PSSession`, `Specifies a PowerShell user-managed session ( PSSession ) from which this cmdlet import modules into the current session. Enter a variable that contains a PSSession or a command that gets a PSSession , such as a Get-PSSession command.

When you import a module from a different session into the current session, you can use the cmdlets from the module in the current session, just as you would use cmdlets from a local module. Commands that use the remote cmdlets actually run in the remote session, but the remoting details are managed in the background by PowerShell.

This parameter uses the Implicit Remoting feature of PowerShell. It is equivalent to using the Import-PSSession cmdlet to import particular modules from a session. Import-Module cannot import PowerShell Core modules from another session. The PowerShell Core modules have names that begin with Microsoft.PowerShell.

This parameter was introduced in Windows PowerShell 3.0.`, `PSSession`),
        new Parameter(`PassThru`, `Returns an object representing the item with which you are working. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Prefix`, `Specifies a prefix that this cmdlet adds to the nouns in the names of imported module members.

Use this parameter to avoid name conflicts that might occur when different members in the session have the same name. This parameter does not change the module, and it does not affect files that the module imports for its own use. These are known as nested modules. This cmdlet affects only the names of members in the current session.

For example, if you specify the prefix UTC and then import a Get-Date cmdlet, the cmdlet is known in the session as Get-UTCDate , and it is not confused with the original Get-Date cmdlet.

The value of this parameter takes precedence over the DefaultCommandPrefix property of the module, which specifies the default prefix.`, `String`),
        new Parameter(`RequiredVersion`, `Specifies a version of the module that this cmdlet imports. If the version is not installed, Import-Module generates an error.

By default, Import-Module imports the module without checking the version number.

To specify a minimum version, use the MinimumVersion parameter. You can also use the Module and Version parameters of the #Requires keyword to require a specific version of a module in a script.

This parameter was introduced in Windows PowerShell 3.0.

Scripts that use RequiredVersion to import modules that are included with existing releases of the Windows operating system do not automatically run in future releases of the Windows operating system. This is because PowerShell module version numbers in future releases of the Windows operating system are higher than module version numbers in existing releases of the Windows operating system.`, `Version`),
        new Parameter(`Scope`, `Specifies a scope into which this cmdlet imports the module.

The acceptable values for this parameter are:

- Global . Available to all commands in the session. Equivalent to the Global parameter. - Local . Available only in the current scope.

By default, when Import-Module cmdlet is called from the command prompt, script file, or scriptblock, all the commands are imported into the global session state. You can use the -Scope parameter with the value of Local to import module content into the script or scriptblock scope.

When invoked from another module, Import-Module cmdlet imports the commands in a module, including commands from nested modules, into the caller's session state. Specifying -Scope Global or -Global indicates that this cmdlet imports modules into the global session state so they are available to all commands in the session.

The Global parameter is equivalent to the Scope parameter with a value of Global.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`Variable`, `Specifies an array of variables that this cmdlet imports from the module into the current session. Enter a list of variables. Wildcard characters are permitted.

Some modules automatically export selected variables into your session when you import the module. This parameter lets you select from among the exported variables.`, `String[]`),
        new Parameter(`MaximumVersion`, `Specifies a maximum version. This cmdlet imports only a version of the module that is less than or equal to the specified value. If no version qualifies, Import-Module generates an error.`, `String`),
    ], `Adds modules to the current session.`, `Import-Module [-Assembly] <Assembly[]> [-Alias <String[]>] [-ArgumentList <Object[]>] [-AsCustomObject] [-Cmdlet <String[]>] [-DisableNameChecking] [-Force] [-Function <String[]>] [-Global] [-NoClobber] [-PassThru] [-Prefix <String>] [-Scope {Local | Global}] [-Variable <String[]>] [<CommonParameters>]

Import-Module [-Name] <String[]> [-Alias <String[]>] [-ArgumentList <Object[]>] [-AsCustomObject] [-CimNamespace <String>] [-CimResourceUri <Uri>] -CimSession <CimSession> [-Cmdlet <String[]>] [-DisableNameChecking] [-Force] [-Function <String[]>] [-Global] [-MinimumVersion <Version>] [-NoClobber] [-PassThru] [-Prefix <String>] [-RequiredVersion <Version>] [-Scope {Local | Global}] [-Variable <String[]>] [-MaximumVersion <String>] [<CommonParameters>]

Import-Module [-FullyQualifiedName] <ModuleSpecification[]> [-Alias <String[]>] [-ArgumentList <Object[]>] [-AsCustomObject] [-Cmdlet <String[]>] [-DisableNameChecking] [-Force] [-Function <String[]>] [-Global] [-NoClobber] [-PassThru] [-Prefix <String>] [-Scope {Local | Global}] [-Variable <String[]>] [<CommonParameters>]

Import-Module [-FullyQualifiedName] <ModuleSpecification[]> [-Alias <String[]>] [-ArgumentList <Object[]>] [-AsCustomObject] [-Cmdlet <String[]>] [-DisableNameChecking] [-Force] [-Function <String[]>] [-Global] [-NoClobber] -PSSession <PSSession> [-PassThru] [-Prefix <String>] [-Scope {Local | Global}] [-Variable <String[]>] [<CommonParameters>]

Import-Module [-Name] <String[]> [-Alias <String[]>] [-ArgumentList <Object[]>] [-AsCustomObject] [-Cmdlet <String[]>] [-DisableNameChecking] [-Force] [-Function <String[]>] [-Global] [-MinimumVersion <Version>] [-NoClobber] [-PassThru] [-Prefix <String>] [-RequiredVersion <Version>] [-Scope {Local | Global}] [-Variable <String[]>] [-MaximumVersion <String>] [<CommonParameters>]

Import-Module [-Name] <String[]> [-Alias <String[]>] [-ArgumentList <Object[]>] [-AsCustomObject] [-Cmdlet <String[]>] [-DisableNameChecking] [-Force] [-Function <String[]>] [-Global] [-MinimumVersion <Version>] [-NoClobber] -PSSession <PSSession> [-PassThru] [-Prefix <String>] [-RequiredVersion <Version>] [-Scope {Local | Global}] [-Variable <String[]>] [-MaximumVersion <String>] [<CommonParameters>]

Import-Module [-ModuleInfo] <PSModuleInfo[]> [-Alias <String[]>] [-ArgumentList <Object[]>] [-AsCustomObject] [-Cmdlet <String[]>] [-DisableNameChecking] [-Force] [-Function <String[]>] [-Global] [-NoClobber] [-PassThru] [-Prefix <String>] [-Scope {Local | Global}] [-Variable <String[]>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Import-PackageProvider`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Import-PowerShellDataFile`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Import-PSSession`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Install-Package`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Install-PackageProvider`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Invoke-CimMethod`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Invoke-Command`, [
        new Parameter(`AllowRedirection`, `Allows redirection of this connection to an alternate Uniform Resource Identifier (URI).

When you use the ConnectionURI parameter, the remote destination can return an instruction to redirect to a different URI. By default, PowerShell does not redirect connections, but you can use this parameter to allow it to redirect the connection.

You can also limit the number of times the connection is redirected by changing the MaximumConnectionRedirectionCount session option value. Use the MaximumRedirection parameter of the New-PSSessionOption cmdlet or set the MaximumConnectionRedirectionCount property of the $PSSessionOption preference variable. The default value is 5.`, `SwitchParameter`),
        new Parameter(`ApplicationName`, `Specifies the application name segment of the connection URI. Use this parameter to specify the application name when you are not using the ConnectionURI parameter in the command.

The default value is the value of the $PSSessionApplicationName preference variable on the local computer. If this preference variable is not defined, the default value is WSMAN. This value is appropriate for most uses. For more information, see about_Preference_Variables (About/about_Preference_Variables.md).

The WinRM service uses the application name to select a listener to service the connection request. The value of this parameter should match the value of the URLPrefix property of a listener on the remote computer.`, `String`),
        new Parameter(`ArgumentList`, `Supplies the values of local variables in the command. The variables in the command are replaced by these values before the command is run on the remote computer. Enter the values in a comma-separated list. Values are associated with variables in the order that they are listed. The alias for ArgumentList is Args.

The values in the ArgumentList parameter can be actual values, such as 1024, or they can be references to local variables, such as $max.

To use local variables in a command, use the following command format:

"{param($<name1>[, $<name2>]...) <command-with-local-variables>} -ArgumentList <value> -or- <local-variable>"

The param keyword lists the local variables that are used in the command. ArgumentList supplies the values of the variables, in the order that they are listed.`, `Object[]`),
        new Parameter(`AsJob`, `Indicates that this cmdlet runs the command as a background job on a remote computer. Use this parameter to run commands that take an extensive time to finish.

When you use the AsJob parameter, the command returns an object that represents the job, and then displays the command prompt. You can continue to work in the session while the job finishes. To manage the job, use the Job cmdlets. To get the job results, use the Receive-Job cmdlet.

The AsJob parameter resembles using the Invoke-Command cmdlet to run a Start-Job command remotely. However, with AsJob , the job is created on the local computer, even though the job runs on a remote computer, and the results of the remote job are automatically returned to the local computer.

For more information about PowerShell background jobs, see about_Jobs (About/about_Jobs.md) and [about_Remote_Jobs](About/about_Remote_Jobs.md).`, `SwitchParameter`),
        new Parameter(`Authentication`, `Specifies the mechanism that is used to authenticate the user's credentials. The acceptable values for this parameter are:

- Default

- Basic

- Credssp

- Digest

- Kerberos

- Negotiate

- NegotiateWithImplicitCredential



The default value is Default.

CredSSP authentication is available only in Windows Vista, Windows Server 2008, and later versions of the Windows operating system.

For more information about the values of this parameter, see AuthenticationMechanism Enumeration (https://msdn.microsoft.com/library/system.management.automation.runspaces.authenticationmechanism)in the MSDN library.

CAUTION: Credential Security Support Provider (CredSSP) authentication, in which the user's credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.`, `AuthenticationMechanism`),
        new Parameter(`CertificateThumbprint`, `Specifies the digital public key certificate (X509) of a user account that has permission to connect to the disconnected session. Enter the certificate thumbprint of the certificate.

Certificates are used in client certificate-based authentication. They can be mapped only to local user accounts; they do not work with domain accounts.

To get a certificate thumbprint, use a Get-Item or Get-ChildItem command in the PowerShell Cert: drive.`, `String`),
        new Parameter(`ComputerName`, `Specifies the computers on which the command runs. The default is the local computer.

When you use the ComputerName parameter, PowerShell creates a temporary connection that is used only to run the specified command and is then closed. If you need a persistent connection, use the Session parameter.

Type the NETBIOS name, IP address, or fully qualified domain name of one or more computers in a comma-separated list. To specify the local computer, type the computer name, localhost, or a dot (.).

To use an IP address in the value of ComputerName , the command must include the Credential parameter. Also, the computer must be configured for HTTPS transport or the IP address of the remote computer must be included in the WinRM TrustedHosts list on the local computer. For instructions for adding a computer name to the TrustedHosts list, see "How to Add a Computer to the Trusted Host List" in about_Remote_Troubleshooting.

On Windows Vista and later versions of the Windows operating system, to include the local computer in the value of ComputerName , you must open PowerShell by using the Run as administrator option.`, `String[]`),
        new Parameter(`ConfigurationName`, `Specifies the session configuration that is used for the new PSSession .

Enter a configuration name or the fully qualified resource URI for a session configuration. If you specify only the configuration name, the following schema URI is prepended: http://schemas.microsoft.com/PowerShell.

The session configuration for a session is located on the remote computer. If the specified session configuration does not exist on the remote computer, the command fails.

The default value is the value of the $PSSessionConfigurationName preference variable on the local computer. If this preference variable is not set, the default is Microsoft.PowerShell. For more information, see about_Preference_Variables.`, `String`),
        new Parameter(`ConnectionUri`, `Specifies a URI that defines the connection endpoint of the session. The URI must be fully qualified.

The format of this string is as follows:

"<Transport>://<ComputerName>:<Port>/<ApplicationName>"

The default value is as follows:

"http://localhost:5985/WSMAN"

If you do not specify a connection URI, you can use the UseSSL and Port parameters to specify the connection URI values.

Valid values for the Transport segment of the URI are HTTP and HTTPS. If you specify a connection URI with a Transport segment, but do not specify a port, the session is created with standards ports: 80 for HTTP and 443 for HTTPS. To use the default ports for PowerShell remoting, specify port 5985 for HTTP or 5986 for HTTPS.

If the destination computer redirects the connection to a different URI, PowerShell prevents the redirection unless you use the AllowRedirection parameter in the command.`, `Uri[]`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01. Or, enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.`, `PSCredential`),
        new Parameter(`EnableNetworkAccess`, `Indicates that this cmdlet adds an interactive security token to loopback sessions. The interactive token lets you run commands in the loopback session that get data from other computers. For example, you can run a command in the session that copies XML files from a remote computer to the local computer.

A loopback session is a PSSession that originates and ends on the same computer. To create a loopback session, omit the ComputerName parameter or set its value to . (dot), localhost, or the name of the local computer.

By default, loopback sessions are created by using a network token, which might not provide sufficient permission to authenticate to remote computers.

The EnableNetworkAccess parameter is effective only in loopback sessions. If you use EnableNetworkAccess when you create a session on a remote computer, the command succeeds, but the parameter is ignored.

You can also allow remote access in a loopback session by using the CredSSP value of the Authentication parameter, which delegates the session credentials to other computers.

To protect the computer from malicious access, disconnected loopback sessions that have interactive tokens, which are those created by using EnableNetworkAccess , can be reconnected only from the computer on which the session was created. Disconnected sessions that use CredSSP authentication can be reconnected from other computers. For more information, see Disconnect-PSSession.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`FilePath`, `Specifies a local script that this cmdlet runs on one or more remote computers. Enter the path and file name of the script, or pipe a script path to Invoke-Command . The script must reside on the local computer or in a directory that the local computer can access. Use ArgumentList to specify the values of parameters in the script.

When you use this parameter, PowerShell converts the contents of the specified script file to a script block, transmits the script block to the remote computer, and runs it on the remote computer.`, `String`),
        new Parameter(`HideComputerName`, `Indicates that this cmdlet omits the computer name of each object from the output display. By default, the name of the computer that generated the object appears in the display.

This parameter affects only the output display. It does not change the object.`, `SwitchParameter`),
        new Parameter(`InDisconnectedSession`, `Indicates that this cmdlet runs a command or script in a disconnected session.

When you use the InDisconnectedSession parameter, Invoke-Command creates a persistent session on each remote computer, starts the command specified by the ScriptBlock or FilePath parameter, and then disconnects from the session. The commands continue to run in the disconnected sessions. InDisconnectedSession enables you to run commands without maintaining a connection to the remote sessions. Also, because the session is disconnected before any results are returned, InDisconnectedSession makes sure that all command results are returned to the reconnected session, instead of being split between sessions.

You cannot use InDisconnectedSession with the Session parameter or the AsJob parameter.

Commands that use InDisconnectedSession return a PSSession object that represents the disconnected session. They do not return the command output. To connect to the disconnected session, use the Connect-PSSession or Receive-PSSession cmdlets. To get the results of commands that ran in the session, use the Receive-PSSession cmdlet. To run commands that generate output in a disconnected session, set the value of the OutputBufferingMode session option to Drop. If you intend to connect to the disconnected session, set the idle time-out in the session so that it provides sufficient time for you to connect before deleting the session.

You can set the output buffering mode and idle time-out in the SessionOption parameter or in the $PSSessionOption preference variable. For more information about session options, see New-PSSessionOption and about_Preference_Variables.

For more information about the Disconnected Sessions feature, see about_Remote_Disconnected_Sessions.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`InputObject`, `Specifies input to the command. Enter a variable that contains the objects or type a command or expression that gets the objects.

When using the InputObject parameter, use the $Input automatic variable in the value of the ScriptBlock parameter to represent the input objects.`, `PSObject`),
        new Parameter(`JobName`, `Specifies a friendly name for the background job. By default, jobs are named Job<n>, where <n> is an ordinal number.

If you use the JobName parameter in a command, the command is run as a job, and Invoke-Command returns a job object, even if you do not include AsJob in the command.

For more information about PowerShell background jobs, see about_Jobs (About/about_Jobs.md).`, `String`),
        new Parameter(`NoNewScope`, `Indicates that this cmdlet runs the specified command in the current scope. By default, Invoke-Command runs commands in their own scope.

This parameter is valid only in commands that are run in the current session, that is, commands that omit both the ComputerName and Session parameters.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`Port`, `Specifies the network port on the remote computer that is used for this command. To connect to a remote computer, the remote computer must be listening on the port that the connection uses. The default ports are 5985, which is the WinRM port for HTTP, and 5986, which is the WinRM port for HTTPS.

Before using an alternate port, configure the WinRM listener on the remote computer to listen at that port. To configure the listener, type the following two commands at the PowerShell prompt:

"Remove-Item -Path WSMan:\Localhost\listener\listener* -Recurse"

"New-Item -Path WSMan:\Localhost\listener -Transport http -Address * -Port <port-number>"

Do not use the Port parameter unless you must. The port that is set in the command applies to all computers or sessions on which the command runs. An alternate port setting might prevent the command from running on all computers.`, `Int32`),
        new Parameter(`ScriptBlock`, `Specifies the commands to run. Enclose the commands in braces ( { } ) to create a script block. This parameter is required.

By default, any variables in the command are evaluated on the remote computer. To include local variables in the command, use ArgumentList .`, `ScriptBlock`),
        new Parameter(`Session`, `Specifies an array of sessions in which this cmdlet runs the command. Enter a variable that contains PSSession objects or a command that creates or gets the PSSession objects, such as a New-PSSession or Get-PSSession command.

When you create a PSSession , PowerShell establishes a persistent connection to the remote computer. Use a PSSession to run a series of related commands that share data. To run a single command or a series of unrelated commands, use the ComputerName parameter. For more information, see about_PSSessions.`, `PSSession[]`),
        new Parameter(`SessionName`, `Specifies a friendly name for a disconnected session. You can use the name to refer to the session in subsequent commands, such as a Get-PSSession command. This parameter is valid only with the InDisconnectedSession parameter.

This parameter was introduced in Windows PowerShell 3.0.`, `String[]`),
        new Parameter(`SessionOption`, `Specifies advanced options for the session. Enter a SessionOption object, such as one that you create by using the New-PSSessionOption cmdlet, or a hash table in which the keys are session option names and the values are session option values.

The default values for the options are determined by the value of the $PSSessionOption preference variable, if it is set. Otherwise, the default values are established by options set in the session configuration.

The session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration.

For a description of the session options that includes the default values, see New-PSSessionOption . For information about the $PSSessionOption preference variable, see about_Preference_Variables (About/about_Preference_Variables.md). For more information about session configurations, see about_Session_Configurations (About/about_Session_Configurations.md).`, `PSSessionOption`),
        new Parameter(`ThrottleLimit`, `Specifies the maximum number of concurrent connections that can be established to run this command. If you omit this parameter or enter a value of 0, the default value, 32, is used.

The throttle limit applies only to the current command, not to the session or to the computer.`, `Int32`),
        new Parameter(`UseSSL`, `Indicates that this cmdlet uses the Secure Sockets Layer (SSL) protocol to establish a connection to the remote computer. By default, SSL is not used.

WS-Management encrypts all PowerShell content transmitted over the network. The UseSSL parameter is an additional protection that sends the data across an HTTPS, instead of HTTP.

If you use this parameter, but SSL is not available on the port that is used for the command, the command fails.`, `SwitchParameter`),
        new Parameter(`VMName`, `Specifies an array of names of virtual machines.`, `String[]`),
        new Parameter(`ContainerId`, `Specifies an array of container IDs.`, `String[]`),
        new Parameter(`RunAsAdministrator`, `Indicates that this cmdlet invokes a command as an Administrator.`, `SwitchParameter`),
        new Parameter(`VMId`, `Specifies an array of IDs of virtual machines.`, `Guid[]`),
        new Parameter(`HostName`, `Specifies an array of computer names for a Secure Shell (SSH) based connection. This is similar to the ComputerName parameter except that the connection to the remote computer is made using SSH rather than Windows WinRM.

This parameter was introduced in PowerShell 6.0.`, `String[]`),
        new Parameter(`KeyFilePath`, `Specifies a key file path used by Secure Shell (SSH) to authenticate a user on a remote computer.

SSH allows user authentication to be performed via private/public keys as an alternative to basic password authentication. If the remote computer is configured for key authentication then this parameter can be used to provide the key that identifies the user.

This parameter was introduced in PowerShell 6.0.`, `String`),
        new Parameter(`SSHTransport`, `Indicates that the remote connection is established using Secure Shell (SSH).

By default PowerShell uses Windows WinRM to connect to a remote computer. This switch forces PowerShell to use the HostName parameter set for establishing an SSH based remote connection.

This parameter was introduced in PowerShell 6.0.`, `SwitchParameter`),
        new Parameter(`UserName`, `Specifies the user name for the account used to run a command on the remote computer. User authentication method will depend on how Secure Shell (SSH) is configured on the remote computer.

If SSH is configured for basic password authentication then you will be prompted for the user password.

If SSH is configured for key based user authentication then a key file path can be provided via the KeyFilePath parameter and no password prompt will occur. Note that if the client user key file is located in an SSH known location then the KeyFilePath parameter is not needed for key based authentication, and user authentication will occur automatically based on the user name. See SSH documentation about key based user authentication for more information.

This is not a required parameter. If no UserName parameter is specified then the current log on user name is used for the connection.

This parameter was introduced in PowerShell 6.0.`, `String`),
        new Parameter(`SSHConnection`, `This parameter takes an array of hashtables where each hashtable contains one or more connection parameters needed to establish a Secure Shell (SSH) connection (HostName, Port, UserName, KeyFilePath).

The hashtable connection parameters are the same as defined for the HostName parameter set.

The SSHConnection parameter is useful for creating multiple sessions where each session requires different connection information.

This parameter was introduced in PowerShell 6.0.`, `hashtable`),
    ], `Runs commands on local and remote computers.`, `Invoke-Command [[-ConnectionUri] <Uri[]>] [-ScriptBlock] <ScriptBlock> [-AllowRedirection] [-ArgumentList <Object[]>] [-AsJob] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Credential <PSCredential>] [-EnableNetworkAccess] [-HideComputerName] [-InDisconnectedSession] [-InputObject <PSObject>] [-JobName <String>] [-SessionOption <PSSessionOption>] [-ThrottleLimit <Int32>] [<CommonParameters>]

Invoke-Command [[-ConnectionUri] <Uri[]>] [-FilePath] <String> [-AllowRedirection] [-ArgumentList <Object[]>] [-AsJob] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-ConfigurationName <String>] [-Credential <PSCredential>] [-EnableNetworkAccess] [-HideComputerName] [-InDisconnectedSession] [-InputObject <PSObject>] [-JobName <String>] [-SessionOption <PSSessionOption>] [-ThrottleLimit <Int32>] [<CommonParameters>]

Invoke-Command [[-ComputerName] <String[]>] [-ScriptBlock] <ScriptBlock> [-ApplicationName <String>] [-ArgumentList <Object[]>] [-AsJob] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Credential <PSCredential>] [-EnableNetworkAccess] [-HideComputerName] [-InDisconnectedSession] [-InputObject <PSObject>] [-JobName <String>] [-Port <Int32>] [-SessionName <String[]>] [-SessionOption <PSSessionOption>] [-ThrottleLimit <Int32>] [-UseSSL] [<CommonParameters>]

Invoke-Command [[-ComputerName] <String[]>] [-FilePath] <String> [-ApplicationName <String>] [-ArgumentList <Object[]>] [-AsJob] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-ConfigurationName <String>] [-Credential <PSCredential>] [-EnableNetworkAccess] [-HideComputerName] [-InDisconnectedSession] [-InputObject <PSObject>] [-JobName <String>] [-Port <Int32>] [-SessionName <String[]>] [-SessionOption <PSSessionOption>] [-ThrottleLimit <Int32>] [-UseSSL] [<CommonParameters>]

Invoke-Command [[-Session] <PSSession[]>] [-ScriptBlock] <ScriptBlock> [-ArgumentList <Object[]>] [-AsJob] [-HideComputerName] [-InputObject <PSObject>] [-JobName <String>] [-ThrottleLimit <Int32>] [<CommonParameters>]

Invoke-Command [[-Session] <PSSession[]>] [-FilePath] <String> [-ArgumentList <Object[]>] [-AsJob] [-HideComputerName] [-InputObject <PSObject>] [-JobName <String>] [-ThrottleLimit <Int32>] [<CommonParameters>]

Invoke-Command [-ScriptBlock] <ScriptBlock> [-ArgumentList <Object[]>] [-AsJob] [-ConfigurationName <String>] -Credential <PSCredential> [-HideComputerName] [-InputObject <PSObject>] [-ThrottleLimit <Int32>] -VMId <Guid[]> [<CommonParameters>]

Invoke-Command [-ScriptBlock] <ScriptBlock> [-ArgumentList <Object[]>] [-AsJob] [-ConfigurationName <String>] -Credential <PSCredential> [-HideComputerName] [-InputObject <PSObject>] [-ThrottleLimit <Int32>] -VMName <String[]> [<CommonParameters>]

Invoke-Command [-FilePath] <String> [-ArgumentList <Object[]>] [-AsJob] [-ConfigurationName <String>] -Credential <PSCredential> [-HideComputerName] [-InputObject <PSObject>] [-ThrottleLimit <Int32>] -VMId <Guid[]> [<CommonParameters>]

Invoke-Command [-FilePath] <String> [-ArgumentList <Object[]>] [-AsJob] [-ConfigurationName <String>] -Credential <PSCredential> [-HideComputerName] [-InputObject <PSObject>] [-ThrottleLimit <Int32>] -VMName <String[]> [<CommonParameters>]

Invoke-Command [-ScriptBlock] <ScriptBlock> [-ArgumentList <Object[]>] [-AsJob] [-ConfigurationName <String>] [-HideComputerName] [-InputObject <PSObject>] [-JobName <String>] [-ThrottleLimit <Int32>] -ContainerId <String[]> [-RunAsAdministrator] [<CommonParameters>]

Invoke-Command [-FilePath] <String> [-ArgumentList <Object[]>] [-AsJob] [-ConfigurationName <String>] [-HideComputerName] [-InputObject <PSObject>] [-JobName <String>] [-ThrottleLimit <Int32>] -ContainerId <String[]> [-RunAsAdministrator] [<CommonParameters>]

Invoke-Command [-HostName] <String[]> [-ScriptBlock] <ScriptBlock> [-ArgumentList <Object[]>] [-AsJob] [-HideComputerName] [-InputObject <PSObject>] [-Port <Int32>] [-KeyFilePath <String>] [-SSHTransport] [-UserName <String>] [<CommonParameters>]

Invoke-Command [-FilePath] <String> [-ArgumentList <Object[]>] [-AsJob] [-HideComputerName] [-InputObject <PSObject>] [-Port <Int32>] [<CommonParameters>]

Invoke-Command [-ScriptBlock] <ScriptBlock> [-ArgumentList <Object[]>] [-AsJob] [-HideComputerName] [-InputObject <PSObject>] -SSHConnection <hashtable> [<CommonParameters>]

Invoke-Command [-FilePath] <String> [-ArgumentList <Object[]>] [-AsJob] [-HideComputerName] [-InputObject <PSObject>] [<CommonParameters>]

Invoke-Command [-ScriptBlock] <ScriptBlock> [-ArgumentList <Object[]>] [-InputObject <PSObject>] [-NoNewScope] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Invoke-Expression`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Invoke-History`, [
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Id`, `Specifies the ID of a command in the history. You can type the ID number of the command or the first few characters of the command.

If you type characters, Invoke-History matches the most recent commands first. If you omit this parameter, Invoke-History runs the last, or most recent, command. To find the ID number of a command, use the Get-History cmdlet.`, `String`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Runs commands from the session history.`, `Invoke-History [[-Id] <String>] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Invoke-Item`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies, as a string array, an item or items that this cmdlet excludes from the operation. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when retrieving the objects rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Include`, `Specifies, as a string array, an item or items that this cmdlet includes in the operation. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies a path to the item. Unlike the Path parameter, the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Path`, `Specifies the path to the selected item.`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Performs the default action on the specified item.`, `Invoke-Item [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Include <String[]>] -LiteralPath <String[]> [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Invoke-Item [-Path] <String[]> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Include <String[]>] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Invoke-RestMethod`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Invoke-WebRequest`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Invoke-WSManAction`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Join-Path`, [
        new Parameter(`AdditionalChildPath`, `Specifies additional elements to append to the value of the Path parameter. The "ChildPath" parameter is still mandatory and must be specified as well.

This parameter is specified with the "ValueFromRemainingArguments" property which enables joining an indefinite number of paths.`, `String[]`),
        new Parameter(`ChildPath`, `Specifies the elements to append to the value of the "Path" parameter. Wildcards are permitted. The "ChildPath" parameter is required, although the parameter name ("ChildPath") is optional.`, `String`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01. Or, enter a "PSCredential" object, such as one generated by the "Get-Credential" cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Path`, `Specifies the main path (or paths) to which the child-path is appended. Wildcards are permitted.

The value of "Path" determines which provider joins the paths and adds the path delimiters. The "Path" parameter is required, although the parameter name ("Path") is optional.`, `String[]`),
        new Parameter(`Resolve`, `Indicates that this cmdlet should attempt to resolve the joined path from the current provider.

- If wildcards are used, the cmdlet returns all paths that match the joined path.

- If no wildcards are used, the cmdlet will error if the path does not exist.`, `SwitchParameter`),
    ], `Combines a path and a child path into a single path.`, `Join-Path [-Path] <String[]> [-ChildPath] <String> [[-AdditionalChildPath] <String[]>] [-Credential <PSCredential>] [-Resolve] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Measure-Command`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Measure-Object`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Move-Item`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Destination`, `Specifies the path to the location where the items are being moved. The default is the current directory. Wildcards are permitted, but the result must specify a single location.

To rename the item being moved, specify a new name in the value of the Destination parameter.`, `String`),
        new Parameter(`Exclude`, `Specifies, as a string array, an item or items that this cmdlet excludes from the operation. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when the cmdlet gets the objects, rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Forces the command to run without asking for user confirmation.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies, as a string array, an item or items that this cmdlet moves in the operation. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies the path to the current location of the items. Unlike the Path parameter, the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`PassThru`, `Returns an object representing the item with which you are working. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies the path to the current location of the items. The default is the current directory. Wildcards are permitted.`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Moves an item from one location to another.`, `Move-Item [[-Destination] <String>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Move-Item [-Path] <String[]> [[-Destination] <String>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Move-ItemProperty`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Destination`, `Specifies the path to the destination location.`, `String`),
        new Parameter(`Exclude`, `Specifies, as a string array, a property or property that this cmdlet excludes from the operation. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the provider's format or language. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcards, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when this cmdlet gets the objects rather than having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Forces the command to run without asking for user confirmation.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies, as a string array, a property or property that this cmdlet moves in the operation. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcards are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies the path to the current location of the property. Unlike the Path parameter, the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Name`, `Specifies the name of the property to be moved.`, `String[]`),
        new Parameter(`PassThru`, `Returns an object representing the item with which you are working. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies the path to the current location of the property. Wildcards are permitted.`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Moves a property from one location to another.`, `Move-ItemProperty [-Destination] <String> [-Name] <String[]> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Move-ItemProperty [-Path] <String[]> [-Destination] <String> [-Name] <String[]> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-Alias`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-CimInstance`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-CimSession`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-CimSessionOption`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-Event`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-FileCatalog`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-Guid`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-Item`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Force`, `Forces this cmdlet to create an item that writes over an existing read-only item. Implementation varies from provider to provider. For more information, see about_Providers. Even using the Force parameter, the cmdlet cannot override security restrictions.`, `SwitchParameter`),
        new Parameter(`ItemType`, `Specifies the provider-specified type of the new item. Starting in Windows PowerShell 5.0, you can create symbolic links by specifying SymbolicLink as the value of this parameter.`, `String`),
        new Parameter(`Name`, `Specifies the name of the new item.

You can specify the name of the new item in the Name or Path parameter value, and you can specify the path of the new item in Name or Path value.`, `String`),
        new Parameter(`Path`, `Specifies the path of the location of the new item. Wildcard characters are permitted.

You can specify the name of the new item in Name , or include it in Path .`, `String[]`),
        new Parameter(`Value`, `Specifies the value of the new item. You can also pipe a value to New-Item .`, `Object`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Creates a new item.`, `New-Item [[-Path] <String[]>] [-Credential <PSCredential>] [-Force] [-ItemType <String>] -Name <String> [-Value <Object>] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

New-Item [-Path] <String[]> [-Credential <PSCredential>] [-Force] [-ItemType <String>] [-Value <Object>] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-ItemProperty`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the "Get-Credential" cmdlet. If you type a user name, this cmdlet prompts you for a password.

This parameter is not supported by any providers installed with Windows PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies items that this cmdlet omits. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the format or language of the provider. The value of this parameter qualifies the "Path" parameter.

The syntax of the filter, including the use of wildcard characters, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when it retrieves the objects instead of having Windows PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Forces the cmdlet to create a property on an object that cannot otherwise be accessed by the user. Implementation varies from provider to provider. For more information, see about_Providers.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies items that this cmdlet includes. The value of this parameter qualifies the "Path" parameter. Enter a path element or pattern, such as "*.txt". Wildcard characters are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies a path of the item property. The value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Name`, `Specifies a name for the new property. If the property is a registry entry, this parameter specifies the name of the entry.`, `String`),
        new Parameter(`Path`, `Specifies the path of the item. This parameter identifies the item to which this cmdlet adds the new property.`, `String[]`),
        new Parameter(`PropertyType`, `Specifies the type of property that this cmdlet adds. The acceptable values for this parameter are:

- String.   Specifies a null-terminated string.   Equivalent to REG_SZ. - ExpandString.   Specifies a null-terminated string that contains unexpanded references to environment variables that are expanded when the value is retrieved.   Equivalent to REG_EXPAND_SZ. - Binary.   Specifies binary data in any form.   Equivalent to REG_BINARY. - DWord.   Specifies a 32-bit binary number.   Equivalent to REG_DWORD. - MultiString.   Specifies an array of null-terminated strings terminated by two null characters.   Equivalent to REG_MULTI_SZ. - Qword.   Specifies a 64-bit binary number.   Equivalent to REG_QWORD. - Unknown.   Indicates an unsupported registry data type, such as REG_RESOURCE_LIST.`, `String`),
        new Parameter(`Value`, `Specifies the property value. If the property is a registry entry, this parameter specifies the value of the entry.`, `Object`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Creates a new property for an item and sets its value.`, `New-ItemProperty [-Name] <String> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-PropertyType <String>] [-Value <Object>] [-Confirm] [-WhatIf] [<CommonParameters>]

New-ItemProperty [-Path] <String[]> [-Name] <String> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-PropertyType <String>] [-Value <Object>] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-Module`, [
        new Parameter(`ArgumentList`, `Specifies an array of arguments which are parameter values that are passed to the script block.`, `Object[]`),
        new Parameter(`AsCustomObject`, `Indicates that this cmdlet returns a custom object that represents the dynamic module. The module members are implemented as script methods of the custom object, but they are not imported into the session. You can save the custom object in a variable and use dot notation to invoke the members.

If the module has multiple members with the same name, such as a function and a variable that are both named A, only one member with each name can be accessed from the custom object.`, `SwitchParameter`),
        new Parameter(`Cmdlet`, `Specifies an array of cmdlets that this cmdlet exports from the module into the current session. Enter a comma-separated list of cmdlets. Wildcard characters are permitted. By default, all cmdlets in the module are exported.

You cannot define cmdlets in a script block, but a dynamic module can include cmdlets if it imports the cmdlets from a binary module.`, `String[]`),
        new Parameter(`Function`, `Specifies an array of functions that this cmdlet exports from the module into the current session. Enter a comma-separated list of functions. Wildcard characters are permitted. By default, all functions defined in a module are exported.`, `String[]`),
        new Parameter(`Name`, `Specifies a name for the new module. You can also pipe a module name to New-Module.

The default value is an autogenerated name that starts with " _DynamicModule " and is followed by a GUID that specifies the path of the dynamic module.`, `String`),
        new Parameter(`ReturnResult`, `Indicates that this cmdlet runs the script block and returns the script block results instead of returning a module object.`, `SwitchParameter`),
        new Parameter(`ScriptBlock`, `Specifies the contents of the dynamic module. Enclose the contents in braces ( { } ) to create a script block. This parameter is required.`, `ScriptBlock`),
    ], `Creates a new dynamic module that exists only in memory.`, `New-Module [-Name] <String> [-ScriptBlock] <ScriptBlock> [-ArgumentList <Object[]>] [-AsCustomObject] [-Cmdlet <String[]>] [-Function <String[]>] [-ReturnResult] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-ModuleManifest`, [
        new Parameter(`AliasesToExport`, `Specifies the aliases that the module exports. Wildcard characters are permitted.

You can use this parameter to restrict the aliases that are exported by the module. It can remove aliases from the list of exported aliases, but it cannot add aliases to the list.

If you omit this parameter, New-ModuleManifest creates an AliasesToExport key with a value of * (all), meaning that all aliases that are exported by the module are exported by the manifest.`, `String[]`),
        new Parameter(`Author`, `Specifies the module author.

If you omit this parameter, New-ModuleManifest creates an Author key with the name of the current user.`, `String`),
        new Parameter(`ClrVersion`, `Specifies the minimum version of the Common Language Runtime (CLR) of the Microsoft .NET Framework that the module requires.`, `Version`),
        new Parameter(`CmdletsToExport`, `Specifies the cmdlets that the module exports. Wildcard characters are permitted.

You can use this parameter to restrict the cmdlets that are exported by the module. It can remove cmdlets from the list of exported cmdlets, but it cannot add cmdlets to the list.

If you omit this parameter, New-ModuleManifest creates a CmdletsToExport key with a value of * (all), meaning that all cmdlets that are exported by the module are exported by the manifest.`, `String[]`),
        new Parameter(`CompanyName`, `Identifies the company or vendor who created the module.

If you omit this parameter, New-ModuleManifest creates a CompanyName key with a value of "Unknown".`, `String`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Copyright`, `Specifies a copyright statement for the module.

If you omit this parameter, New-ModuleManifest creates a Copyright key with a value of  "(c) <year> <username>. All rights reserved." where <year> is the current year and <username> is the value of the Author key (if one is specified) or the name of the current user.`, `String`),
        new Parameter(`DefaultCommandPrefix`, `Specifies a prefix that is prepended to the nouns of all commands in the module when they are imported into a session. Prefixes prevent command name conflicts in a user's session.

Module users can override this prefix by specifying the Prefix parameter of the Import-Module cmdlet.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`Description`, `Describes the contents of the module.`, `String`),
        new Parameter(`DotNetFrameworkVersion`, `Specifies the minimum version of the Microsoft .NET Framework that the module requires.`, `Version`),
        new Parameter(`DscResourcesToExport`, ``, `String[]`),
        new Parameter(`FileList`, `Specifies all items that are included in the module.

This key is designed to act as a module inventory. The files listed in the key are not automatically exported with the module.`, `String[]`),
        new Parameter(`FormatsToProcess`, `Specifies the formatting files (.ps1xml) that run when the module is imported.

When you import a module, PowerShell runs the Update-FormatData cmdlet with the specified files. Because formatting files are not scoped, they affect all session states in the session.`, `String[]`),
        new Parameter(`FunctionsToExport`, `Specifies the functions that the module exports. Wildcard characters are permitted.

You can use this parameter to restrict the functions that are exported by the module. It can remove functions from the list of exported aliases, but it cannot add functions to the list.

If you omit this parameter, New-ModuleManifest creates an FunctionsToExport key with a value of * (all), meaning that all functions that are exported by the module are exported by the manifest.`, `String[]`),
        new Parameter(`Guid`, `Specifies a unique identifier for the module. The GUID can be used to distinguish among modules with the same name.

If you omit this parameter, New-ModuleManifest creates a GUID key in the manifest and generates a GUID for the value.

To create a new GUID in PowerShell, type "[guid]::NewGuid()".`, `Guid`),
        new Parameter(`HelpInfoUri`, `Specifies the Internet address of the HelpInfo XML file for the module. Enter an Uniform Resource Identifier (URI) that starts with "http" or "https".

The HelpInfo XML file supports the Updatable Help feature that was introduced in Windows PowerShell 3.0. It contains information about the location of downloadable help files for the module and the version numbers of the newest help files for each supported locale. For information about Updatable Help, see about_Updatable_Help (http://go.microsoft.com/fwlink/?LinkID=235801). For information about the HelpInfo XML file, see "Supporting Updatable Help" in the Microsoft Developer Network (MSDN) library.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`IconUri`, ``, `Uri`),
        new Parameter(`LicenseUri`, ``, `Uri`),
        new Parameter(`ModuleList`, `Lists all modules that are included in this module.

Enter each module name as a string or as a hash table with ModuleName and ModuleVersion keys. The hash table can also have an optional GUID key. You can combine strings and hash tables in the parameter value. For more information, see the examples.

This key is designed to act as a module inventory. The modules that are listed in the value of this key are not automatically processed.`, `Object[]`),
        new Parameter(`ModuleVersion`, `Specifies the version of the module.

This parameter is not required by the cmdlet, but a ModuleVersion key is required in the manifest. If you omit this parameter, New-ModuleManifest creates a ModuleVersion key with a value of "1.0".`, `Version`),
        new Parameter(`NestedModules`, `Specifies script modules (.psm1) and binary modules (.dll) that are imported into the module's session state. The files in the NestedModules key run in the order in which they are listed in the value.

Enter each module name as a string or as a hash table with ModuleName and ModuleVersion keys. The hash table can also have an optional GUID key. You can combine strings and hash tables in the parameter value. For more information, see the examples.

Typically, nested modules contain commands that the root module needs for its internal processing. By default, the commands in nested modules are exported from the module's session state into the caller's session state, but the root module can restrict the commands that it exports, for example, by using an Export-ModuleMember command.

Nested modules in the module session state are available to the root module, but they are not returned by a Get-Module command in the caller's session state.

Scripts (.ps1) that are listed in the NestedModules key are run in the module's session state, not in the caller's session state. To run a script in the caller's session state, list the script file name in the value of the ScriptsToProcess key in the manifest.`, `Object[]`),
        new Parameter(`PassThru`, `Indicates that this cmdlet writes the resulting module manifest to the console, in addition to creating a .psd1 file. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies the path and file name of the new module manifest. Enter a path and file name with a .psd1 file name extension, such as "$pshome\Modules\MyModule\MyModule.psd1". This parameter is required.

If you specify the path of an existing file, New-ModuleManifest replaces the file without warning unless the file has the read-only attribute.

The manifest should be located in the module's directory, and the manifest file name should be the same as the module directory name, but with a .psd1 file name extension.

You cannot use variables, such as $pshome or $home, in response to a prompt for a Path parameter value. To use a variable, include the Path parameter in the command.`, `String`),
        new Parameter(`PowerShellHostName`, `Specifies the name of the PowerShell host program that the module requires. Enter the name of the host program, such as "Windows PowerShell ISE Host" or "ConsoleHost". Wildcard characters are not permitted.

To find the name of a host program, in the program, type "$host.name".`, `String`),
        new Parameter(`PowerShellHostVersion`, `Specifies the minimum version of the PowerShell host program that works with the module. Enter a version number, such as 1.1.`, `Version`),
        new Parameter(`PowerShellVersion`, `Specifies the minimum version of PowerShell that works with this module. For example, you can enter 3.0, 4.0, or 5.0 as the value of this parameter.`, `Version`),
        new Parameter(`PrivateData`, `Specifies data that is passed to the module when it is imported.`, `Object`),
        new Parameter(`ProcessorArchitecture`, `Specifies the processor architecture that the module requires. The acceptable values for this parameter are: x86, AMD64, IA64, and None. None indicates unknown or unspecified.`, `ProcessorArchitecture`),
        new Parameter(`ProjectUri`, ``, `Uri`),
        new Parameter(`ReleaseNotes`, `Specifies release notes.`, `String`),
        new Parameter(`RequiredAssemblies`, `Specifies the assembly (.dll) files that the module requires. Enter the assembly file names. PowerShell loads the specified assemblies before updating types or formats, importing nested modules, or importing the module file that is specified in the value of the RootModule key.

Use this parameter to list all the assemblies that the module requires. This includes assemblies that must be loaded to update any formatting or type files that are listed in the FormatsToProcess or TypesToProcess keys, even if those assemblies are also listed as binary modules in the NestedModules key.`, `String[]`),
        new Parameter(`RequiredModules`, `Specifies modules that must be in the global session state. If the required modules are not in the global session state, PowerShell imports them. If the required modules are not available, the Import-Module command fails.

Enter each module name as a string or as a hash table with ModuleName and ModuleVersion keys. The hash table can also have an optional GUID key. You can combine strings and hash tables in the parameter value. For more information, see the examples.

In Windows PowerShell 2.0, Import-Module does not import required modules automatically. It just verifies that the required modules are in the global session state.`, `Object[]`),
        new Parameter(`RootModule`, `Specifies the primary or root file of the module. Enter the file name of a script (.ps1), a script module (.psm1), a module manifest (.psd1), an assembly (.dll), a cmdlet definition XML file (.cdxml), or a workflow (.xaml). When the module is imported, the members that are exported from the root module file are imported into the caller's session state.

If a module has a manifest file and no root file has been designated in the RootModule key, the manifest becomes the primary file for the module, and the module becomes a manifest module (ModuleType = Manifest).

To export members from .psm1 or .dll files in a module that has a manifest, the names of those files must be specified in the values of the RootModule or NestedModules keys in the manifest. Otherwise, their members are not exported.

In Windows PowerShell 2.0, this key was called ModuleToProcess . You can use the RootModule parameter name or its ModuleToProcess alias.`, `String`),
        new Parameter(`ScriptsToProcess`, `Specifies script (.ps1) files that run in the caller's session state when the module is imported. You can use these scripts to prepare an environment, just as you might use a logon script.

To specify scripts that run in the module's session state, use the NestedModules key.`, `String[]`),
        new Parameter(`Tags`, ``, `String[]`),
        new Parameter(`TypesToProcess`, `Specifies the type files (.ps1xml) that run when the module is imported.

When you import the module, PowerShell runs the Update-TypeData cmdlet with the specified files. Because type files are not scoped, they affect all session states in the session.`, `String[]`),
        new Parameter(`VariablesToExport`, `Specifies the variables that the module exports. Wildcard characters are permitted.

You can use this parameter to restrict the variables that are exported by the module. It can remove variables from the list of exported variables, but it cannot add variables to the list.

If you omit this parameter, New-ModuleManifest creates a VariablesToExport key with a value of * (all), meaning that all variables that are exported by the module are exported by the manifest.`, `String[]`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`CompatiblePSEditions`, `Specifies the compatible PSEditions of the module. For information about PSEdition, see Modules with compatible PowerShell Editions (https://msdn.microsoft.com/en-us/powershell/gallery/psget/module/modulewithpseditionsupport).`, `String[]`),
    ], `Creates a new module manifest.`, `New-ModuleManifest [-Path] <String> [-AliasesToExport <String[]>] [-Author <String>] [-ClrVersion <Version>] [-CmdletsToExport <String[]>] [-CompanyName <String>] [-Confirm] [-Copyright <String>] [-DefaultCommandPrefix <String>] [-Description <String>] [-DotNetFrameworkVersion <Version>] [-DscResourcesToExport <String[]>] [-FileList <String[]>] [-FormatsToProcess <String[]>] [-FunctionsToExport <String[]>] [-Guid <Guid>] [-HelpInfoUri <String>] [-IconUri <Uri>] [-LicenseUri <Uri>] [-ModuleList <Object[]>] [-ModuleVersion <Version>] [-NestedModules <Object[]>] [-PassThru] [-PowerShellHostName <String>] [-PowerShellHostVersion <Version>] [-PowerShellVersion <Version>] [-PrivateData <Object>] [-ProcessorArchitecture {None | MSIL | X86 | IA64 | Amd64 | Arm}] [-ProjectUri <Uri>] [-ReleaseNotes <String>] [-RequiredAssemblies <String[]>] [-RequiredModules <Object[]>] [-RootModule <String>] [-ScriptsToProcess <String[]>] [-Tags <String[]>] [-TypesToProcess <String[]>] [-VariablesToExport <String[]>] [-WhatIf] [-CompatiblePSEditions {Desktop | Core}] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-Object`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-PSDrive`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the "Get-Credential" cmdlet. If you type a user name, this cmdlet prompts you for a password.

Starting in Windows PowerShell 3.0, when the value of the Root parameter is a UNC path, you can use credentials to create file system drives. This parameter is not supported by all PowerShell providers.`, `PSCredential`),
        new Parameter(`Description`, `Specifies a brief text description of the drive. Type any string.

To see the descriptions of all of the drives in the session, type "Get-PSDrive | Format-Table Name, Description". To see the description of a particular drives, type "(Get-PSDrive <DriveName>).Description".`, `String`),
        new Parameter(`Name`, `Specifies a name for the new drive. For persistent mapped network drives, type a drive letter. For temporary PowerShell drives, type any valid string; you are not limited to drive letters.`, `String`),
        new Parameter(`Persist`, `Indicates that this cmdlet creates a Windows mapped network drive. Mapped network drives are saved in Windows on the local computer. They are persistent, not session-specific, and can be viewed and managed in File Explorer and other tools.

When you scope the command locally, that is, without dot-sourcing, the Persist parameter does not persist the creation of a PSDrive beyond the scope in which you run the command. If you run "New-PSDrive" inside a script, and you want the new drive to persist indefinitely, you must dot-source the script. For best results, to force a new drive to persist, specify Global as the value of the Scope parameterin addition to adding Persist to your command.

The name of the drive must be a letter, such as D or E. The value of Root parameter must be a UNC path of a different computer. The value of the PSProvider parameter must be FileSystem.

To disconnect a Windows mapped network drive, use the "Remove-PSDrive" cmdlet. When you disconnect a Windows mapped network drive, the mapping is permanently deleted from the computer, not just deleted from the current session.

Mapped network drives are specific to a user account. Mapped drives created in elevated sessions or sessions using the credential of another user are not visible in sessions started using different credentials.`, `SwitchParameter`),
        new Parameter(`PSProvider`, `Specifies the PowerShell provider that supports drives of this kind.

For example, if the drive is associated with a network share or file system directory, the PowerShell provider is FileSystem. If the drive is associated with a registry key, the provider is Registry.

Temporary PowerShell drives can be associated with any PowerShell provider. Mapped network drives can be associated only with the FileSystem provider.

To see a list of the providers in your PowerShell session, use the "Get-PSProvider" cmdlet.`, `String`),
        new Parameter(`Root`, `Specifies the data store location to which a PowerShell drive is mapped.

For example, specify a network share, such as \\Server01\Public, a local directory, such as C:\Program Files, or a registry key, such as HKLM:\Software\Microsoft.

Temporary PowerShell drives can be associated with a local or remote location on any supported provider drive. Mapped network drives can be associated only with a file system location on a remote computer.`, `String`),
        new Parameter(`Scope`, `Specifies a scope for the drive. The acceptable values for this parameter are: Global, Local, and Script, or a number relative to the current scope. Scopes number 0 through the number of scopes. The current scope number is 0 and its parent is 1. For more information, see about_Scopes (../Microsoft.PowerShell.Core/About/about_Scopes.md).`, `String`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Creates temporary and persistent mapped network drives.`, `New-PSDrive [-Name] <String> [-PSProvider] <String> [-Root] <String> [-Credential <PSCredential>] [-Description <String>] [-Persist] [-Scope <String>] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-PSRoleCapabilityFile`, [
        new Parameter(`AliasDefinitions`, `Adds the specified aliases to sessions that use the role capability file. Enter a hash table with the following keys:

- Name. Name of the alias. This key is required. - Value. The command that the alias represents. This key is required. - Description. A text string that describes the alias. This key is optional. - Options. Alias options. This key is optional. The default value is None. The acceptable values for this parameter are: None, ReadOnly, Constant, Private, or AllScope.

For example: "@{Name="hlp";Value="Get-Help";Description="Gets help";Options="ReadOnly"}"`, `IDictionary[]`),
        new Parameter(`AssembliesToLoad`, `Specifies the assemblies to load into the sessions that use the role capability file.`, `String[]`),
        new Parameter(`Author`, `Specifies the user that created the role capability file.`, `String`),
        new Parameter(`CompanyName`, `Identifies the company that created the role capability file. The default value is Unknown.`, `String`),
        new Parameter(`Copyright`, `Specifies a copyright for the role capability file. If you omit this parameter, New-PSRoleCapabilityFile generates a copyright statement by using the value of the Author parameter.`, `String`),
        new Parameter(`Description`, `Specifies a description for the role capability file.`, `String`),
        new Parameter(`EnvironmentVariables`, `Specifies the environment variables for sessions that expose this role capability file. Enter a hash table in which the keys are the environment variable names and the values are the environment variable values.

For example: "EnvironmentVariables=@{TestShare="\\Server01\TestShare"}"`, `IDictionary`),
        new Parameter(`FormatsToProcess`, `Specifies the formatting files (.ps1xml) that run in sessions that use the role capability file. The value of this parameter must be a full or absolute path of the formatting files.`, `String[]`),
        new Parameter(`FunctionDefinitions`, `Adds the specified functions to sessions that expose the role capability. Enter a hash table with the following keys:

- Name. Name of the function. This key is required. - ScriptBlock. Function body. Enter a script block. This key is required. - Options. Function options. This key is optional. The default value is None. The acceptable values for this parameter are: are None, ReadOnly, Constant, Private, or AllScope.

For example: "@{Name="Get-PowerShellProcess";ScriptBlock={Get-Process PowerShell};Options="AllScope"}"`, `IDictionary[]`),
        new Parameter(`Guid`, `Specifies a unique identifier for the role capability file. If you omit this parameter, New-PSRoleCapabilityFile generates a GUID for the file. To create a new GUID in PowerShell, type "[guid]::NewGuid()".`, `Guid`),
        new Parameter(`ModulesToImport`, `Specifies the modules that are automatically imported into sessions that use the role capability file. By default, all of the commands in listed modules are visible. When used with VisibleCmdlets or VisibleFunctions , the commands visible from the specified modules can be restricted.

Each module used in the value of this parameter can be represented by a string or by a hash table. A module string consists only of the name of the module. A module hash table can include ModuleName , ModuleVersion , and GUID keys. Only the ModuleName key is required.

For example, the following value consists of a string and a hash table. Any combination of strings and hash tables, in any order, is valid.

""TroubleshootingPack", @{ModuleName="PSDiagnostics"; ModuleVersion="1.0.0.0";GUID="c61d6278-02a3-4618-ae37-a524d40a7f44"}"`, `Object[]`),
        new Parameter(`Path`, `Specifies the path and file name of the role capability file. The file must have a .psrc file name extension.`, `String`),
        new Parameter(`ScriptsToProcess`, `Specifies scripts to add to sessions that use the role capability file. Enter the path and file names of the scripts. The value of this parameter must be a full or absolute path of the script file names.`, `String[]`),
        new Parameter(`TypesToProcess`, `Specifies type files (.ps1xml) to add to sessions that use the role capability file. Enter the type file names. The value of this parameter must be a full or absolute path of the type file names.`, `String[]`),
        new Parameter(`VariableDefinitions`, `Specifies variables to add to sessions that use the role capability file. Enter a hash table with the following keys:

- Name. Name of the variable. This key is required. - Value. Variable value. This key is required. - Options. Variable options. This key is optional. The default value is None. The acceptable values for this parameter are: are None, ReadOnly, Constant, Private, or AllScope.

For example: "@{Name="WarningPreference";Value="SilentlyContinue";Options="AllScope"}"`, `Object`),
        new Parameter(`VisibleAliases`, `Limits the aliases in the session to those aliases specified in the value of this parameter, plus any aliases that you define in the AliasDefinition parameter. Wildcard characters are supported. By default, all aliases that are defined by the PowerShell engine and all aliases that modules export are visible in the session.

For example, to limit the available aliases to gm and gcm use this syntax: "VisibleAliases="gcm", "gp""

When any Visible parameter is included in the role capability file, PowerShell removes the Import-Module cmdlet and its ipmo alias from the session.`, `String[]`),
        new Parameter(`VisibleCmdlets`, `Limits the cmdlets in the session to those specified in the value of this parameter. Wildcard characters and Module Qualified Names are supported.

By default, all cmdlets that the modules in the session export are visible in the session. Use the SessionType and ModulesToImport parameters to determine which modules and snap-ins are imported into the session. If no modules in ModulesToImport expose the cmdlet, New-PSRoleCapabilityFile will try load the appropriate module.

When any Visible parameter is included in the session configuration file, PowerShell removes the Import-Module cmdlet and its ipmo alias from the session.`, `Object[]`),
        new Parameter(`VisibleExternalCommands`, `Limits the external binaries, scripts and commands that can be executed in the session to those specified in the value of this parameter. Wildcard characters are supported.

By default, no external commands are visible in this session.

When any Visible parameter is included in the session configuration file, PowerShell removes the Import-Module cmdlet and its ipmo alias from the session.`, `String[]`),
        new Parameter(`VisibleFunctions`, `Limits the functions in the session to those specified in the value of this parameter, plus any functions that you define in the FunctionDefinitions parameter. Wildcard characters are supported.

By default, all functions exported by modules in the session are visible in that session. Use the SessionType and ModulesToImport parameters to determine which modules are imported into the session.

When any Visible parameter is included in the session configuration file, PowerShell removes the Import-Module cmdlet and its ipmo alias from the session.`, `Object[]`),
        new Parameter(`VisibleProviders`, `Limits the PowerShell providers in the session to those specified in the value of this parameter. Wildcard characters are supported.

By default, all providers exported by a module in the session are visible in the session. Use the SessionType and ModulesToImport parameters to determine which modules are imported into the session.

When any Visible parameter is included in the session configuration file, PowerShell removes the Import-Module cmdlet and its ipmo alias from the session.`, `String[]`),
    ], `Creates a file that defines a set of capabilities to be exposed through a session configuration.`, `New-PSRoleCapabilityFile [-Path] <String> [-AliasDefinitions <IDictionary[]>] [-AssembliesToLoad <String[]>] [-Author <String>] [-CompanyName <String>] [-Copyright <String>] [-Description <String>] [-EnvironmentVariables <IDictionary>] [-FormatsToProcess <String[]>] [-FunctionDefinitions <IDictionary[]>] [-Guid <Guid>] [-ModulesToImport <Object[]>] [-ScriptsToProcess <String[]>] [-TypesToProcess <String[]>] [-VariableDefinitions <Object>] [-VisibleAliases <String[]>] [-VisibleCmdlets <Object[]>] [-VisibleExternalCommands <String[]>] [-VisibleFunctions <Object[]>] [-VisibleProviders <String[]>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-PSSession`, [
        new Parameter(`AllowRedirection`, `Indicates that this cmdlet allows redirection of this connection to an alternate Uniform Resource Identifier (URI).

When you use the ConnectionURI parameter, the remote destination can return an instruction to redirect to a different URI. By default, PowerShell does not redirect connections, but you can use this parameter to enable it to redirect the connection.

You can also limit the number of times the connection is redirected by changing the MaximumConnectionRedirectionCount session option value. Use the MaximumRedirection parameter of the New-PSSessionOption cmdlet or set the MaximumConnectionRedirectionCount property of the $PSSessionOption preference variable. The default value is 5.`, `SwitchParameter`),
        new Parameter(`ApplicationName`, `Specifies the application name segment of the connection URI. Use this parameter to specify the application name when you are not using the ConnectionURI parameter in the command.

The default value is the value of the $PSSessionApplicationName preference variable on the local computer. If this preference variable is not defined, the default value is WSMAN. This value is appropriate for most uses. For more information, see about_Preference_Variables (About/about_Preference_Variables.md).

The WinRM service uses the application name to select a listener to service the connection request. The value of this parameter should match the value of the URLPrefix property of a listener on the remote computer.`, `String`),
        new Parameter(`Authentication`, `Specifies the mechanism that is used to authenticate the user's credentials. The acceptable values for this parameter are:

- Default

- Basic

- Credssp

- Digest

- Kerberos

- Negotiate

- NegotiateWithImplicitCredential



The default value is Default.

For more information about the values of this parameter, see AuthenticationMechanism Enumeration (https://msdn.microsoft.com/library/system.management.automation.runspaces.authenticationmechanism)in the MSDN library.

Caution: Credential Security Support Provider (CredSSP) authentication, in which the user credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.`, `AuthenticationMechanism`),
        new Parameter(`CertificateThumbprint`, `Specifies the digital public key certificate (X509) of a user account that has permission to perform this action. Enter the certificate thumbprint of the certificate.

Certificates are used in client certificate-based authentication. They can be mapped only to local user accounts; they do not work with domain accounts.

To get a certificate, use the Get-Item or Get-ChildItem command in the PowerShell Cert: drive.`, `String`),
        new Parameter(`ComputerName`, `Specifies an array of names of computers. This cmdlet creates a persistent connection ( PSSession ) to the specified computer. If you enter multiple computer names, New-PSSession creates multiple PSSession objects, one for each computer. The default is the local computer.

Type the NetBIOS name, an IP address, or a fully qualified domain name of one or more remote computers. To specify the local computer, type the computer name, localhost, or a dot (.). When the computer is in a different domain than the user, the fully qualified domain name is required. You can also pipe a computer name, in quotation marks, to New-PSSession .

To use an IP address in the value of the ComputerName parameter, the command must include the Credential parameter. Also, the computer must be configured for HTTPS transport or the IP address of the remote computer must be included in the WinRM TrustedHosts list on the local computer. For instructions for adding a computer name to the TrustedHosts list, see "How to Add a Computer to the Trusted Host List" in about_Remote_Troubleshooting (http://go.microsoft.com/fwlink/?LinkID=135188).

To include the local computer in the value of the ComputerName parameter, start PowerShell by using the Run as administrator option.`, `String[]`),
        new Parameter(`ConfigurationName`, `Specifies the session configuration that is used for the new PSSession .

Enter a configuration name or the fully qualified resource URI for a session configuration. If you specify only the configuration name, the following schema URI is prepended: http://schemas.microsoft.com/PowerShell.

The session configuration for a session is located on the remote computer. If the specified session configuration does not exist on the remote computer, the command fails.

The default value is the value of the $PSSessionConfigurationName preference variable on the local computer. If this preference variable is not set, the default is Microsoft.PowerShell. For more information, see about_Preference_Variables (About/about_Preference_Variables.md).`, `String`),
        new Parameter(`ConnectionUri`, `Specifies a URI that defines the connection endpoint for the session. The URI must be fully qualified. The format of this string is as follows:

<Transport>://<ComputerName>:<Port>/<ApplicationName>

The default value is as follows:

"http://localhost:5985/WSMAN"

If you do not specify a ConnectionURI , you can use the UseSSL , ComputerName , Port , and ApplicationName parameters to specify the ConnectionURI values.

Valid values for the Transport segment of the URI are HTTP and HTTPS. If you specify a connection URI with a Transport segment, but do not specify a port, the session is created with standards ports: 80 for HTTP and 443 for HTTPS. To use the default ports for PowerShell remoting, specify port 5985 for HTTP or 5986 for HTTPS.

If the destination computer redirects the connection to a different URI, PowerShell prevents the redirection unless you use the AllowRedirection parameter in the command.`, `Uri[]`),
        new Parameter(`ContainerId`, `Specifies an array of IDs of containers. This cmdlet starts an interactive session with each of the specified containers. To see the containers that are available to you, use the Get-Container cmdlet.`, `String[]`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01, Domain01\User01, or User@Domain.com, or enter a PSCredential object, such as one returned by the Get-Credential cmdlet.

When you type a user name, this cmdlet prompts you for a password.`, `PSCredential`),
        new Parameter(`EnableNetworkAccess`, `Indicates that this cmdlet adds an interactive security token to loopback sessions. The interactive token lets you run commands in the loopback session that get data from other computers. For example, you can run a command in the session that copies XML files from a remote computer to the local computer.

A loopback session is a PSSession that originates and ends on the same computer. To create a loopback session, omit the ComputerName parameter or set its value to dot (.), localhost, or the name of the local computer.

By default, this cmdlet creates loopback sessions by using a network token, which might not provide sufficient permission to authenticate to remote computers.

The EnableNetworkAccess parameter is effective only in loopback sessions. If you use EnableNetworkAccess when you create a session on a remote computer, the command succeeds, but the parameter is ignored.

You can also enable remote access in a loopback session by using the CredSSP value of the Authentication parameter, which delegates the session credentials to other computers.

To protect the computer from malicious access, disconnected loopback sessions that have interactive tokens, which are those created by using the EnableNetworkAccess parameter, can be reconnected only from the computer on which the session was created. Disconnected sessions that use CredSSP authentication can be reconnected from other computers. For more information, see Disconnect-PSSession.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`HostName`, `Specifies an array of computer names for a Secure Shell (SSH) based connection. This is similar to the ComputerName parameter except that the connection to the remote computer is made using SSH rather than Windows WinRM.

This parameter was introduced in PowerShell 6.0.`, `String[]`),
        new Parameter(`KeyFilePath`, `Specifies a key file path used by Secure Shell (SSH) to authenticate a user on a remote computer.

SSH allows user authentication to be performed via private/public keys as an alternative to basic password authentication. If the remote computer is configured for key authentication then this parameter can be used to provide the key that identifies the user.

This parameter was introduced in PowerShell 6.0.`, `String`),
        new Parameter(`Name`, `Specifies a friendly name for the PSSession .

You can use the name to refer to the PSSession when you use other cmdlets, such as Get-PSSession and Enter-PSSession. The name is not required to be unique to the computer or the current session.`, `String[]`),
        new Parameter(`Port`, `Specifies the network port on the remote computer that is used for this connection.

In PowerShell 6.0 this parameter was included in the HostName and SSHConnection parameter sets which support Secure Shell (SSH) connections. WinRM (ComputerName parameter set) To connect to a remote computer, the remote computer must be listening on the port that the connection uses. The default ports are 5985, which is the WinRM port for HTTP, and 5986, which is the WinRM port for HTTPS.

Before using another port, you must configure the WinRM listener on the remote computer to listen at that port. Use the following commands to configure the listener:

1. "winrm delete winrm/config/listener?Address=*+Transport=HTTP"

2. "winrm create winrm/config/listener?Address=*+Transport=HTTP @{Port="<port-number>"}"

Do not use the Port parameter unless you must. The port setting in the command applies to all computers or sessions on which the command runs. An alternate port setting might prevent the command from running on all computers. SSH (HostName and SSHConnection parameter sets) To connect to a remote computer, the remote computer must be configured with the SSH service (SSHD) and must be listening on the port that the connection uses. The default port for SSH is 22.`, `Int32`),
        new Parameter(`RunAsAdministrator`, `Indicates that the PSSession runs as administrator.`, `SwitchParameter`),
        new Parameter(`SSHConnection`, `This parameter takes an array of hashtables where each hashtable contains one or more connection parameters needed to establish a Secure Shell (SSH) connection (HostName, Port, UserName, KeyFilePath).

The hashtable connection parameters are the same as defined for the HostName parameter set.

The SSHConnection parameter is useful for creating multiple sessions where each session requires different connection information.

This parameter was introduced in PowerShell 6.0.`, `Hashtable[]`),
        new Parameter(`SSHTransport`, `Indicates that the remote connection is established using Secure Shell (SSH).

By default PowerShell uses Windows WinRM to connect to a remote computer. This switch forces PowerShell to use the HostName parameter set for establishing an SSH based remote connection.

This parameter was introduced in PowerShell 6.0.`, `SwitchParameter`),
        new Parameter(`Session`, `Specifies an array of PSSession objects that this cmdlet uses as a model for the new PSSession . This parameter creates new PSSession objects that have the same properties as the specified PSSession objects.

Enter a variable that contains the PSSession objects or a command that creates or gets the PSSession objects, such as a New-PSSession or Get-PSSession command.

The resulting PSSession objects have the same computer name, application name, connection URI, port, configuration name, throttle limit, and Secure Sockets Layer (SSL) value as the originals, but they have a different display name, ID, and instance ID (GUID).`, `PSSession[]`),
        new Parameter(`SessionOption`, `Specifies advanced options for the session. Enter a SessionOption object, such as one that you create by using the New-PSSessionOption cmdlet, or a hash table in which the keys are session option names and the values are session option values.

The default values for the options are determined by the value of the $PSSessionOption preference variable, if it is set. Otherwise, the default values are established by options set in the session configuration.

The session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration.

For a description of the session options that includes the default values, see New-PSSessionOption. For information about the $PSSessionOption preference variable, see about_Preference_Variables (About/about_Preference_Variables.md). For more information about session configurations, see about_Session_Configurations (About/about_Session_Configurations.md).`, `PSSessionOption`),
        new Parameter(`ThrottleLimit`, `Specifies the maximum number of concurrent connections that can be established to run this command. If you omit this parameter or enter a value of 0 (zero), the default value, 32, is used.

The throttle limit applies only to the current command, not to the session or to the computer.`, `Int32`),
        new Parameter(`UseSSL`, `Indicates that this cmdlet uses the SSL protocol to establish a connection to the remote computer. By default, SSL is not used.

WS-Management encrypts all PowerShell content transmitted over the network. The UseSSL parameter offers an additional protection that sends the data across an HTTPS connection instead of an HTTP connection.

If you use this parameter, but SSL is not available on the port that is used for the command, the command fails.`, `SwitchParameter`),
        new Parameter(`UserName`, `Specifies the user name for the account used to create a session on the remote computer. User authentication method will depend on how Secure Shell (SSH) is configured on the remote computer.

If SSH is configured for basic password authentication then you will be prompted for the user password.

If SSH is configured for key based user authentication then a key file path can be provided via the KeyFilePath parameter and no password prompt will occur. Note that if the client user key file is located in an SSH known location then the KeyFilePath parameter is not needed for key based authentication, and user authentication will occur automatically based on the user name. See SSH documentation about key based user authentication for more information.

This is not a required parameter. If no UserName parameter is specified then the current log on user name is used for the connection.

This parameter was introduced in PowerShell 6.0.`, `String`),
        new Parameter(`VMId`, `Specifies an array of ID of virtual machines. This cmdlet starts an interactive session with each of the specified virtual machines. To see the virtual machines that are available to you, use the following command:

"Get-VM | Select-Object -Property Name, ID"`, `Guid[]`),
        new Parameter(`VMName`, `Specifies an array of names of virtual machines. This cmdlet starts an interactive session with each of the specified virtual machines. To see the virtual machines that are available to you, use the Get-VM cmdlet.`, `String[]`),
    ], `Creates a persistent connection to a local or remote computer.`, `New-PSSession [-ConnectionUri] <Uri[]> [-AllowRedirection] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Credential <PSCredential>] [-EnableNetworkAccess] [-Name <String[]>] [-SessionOption <PSSessionOption>] [-ThrottleLimit <Int32>] [<CommonParameters>]

New-PSSession [[-ComputerName] <String[]>] [-ApplicationName <String>] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Credential <PSCredential>] [-EnableNetworkAccess] [-Name <String[]>] [-Port <Int32>] [-SessionOption <PSSessionOption>] [-ThrottleLimit <Int32>] [-UseSSL] [<CommonParameters>]

New-PSSession [-ConfigurationName <String>] -Credential <PSCredential> [-Name <String[]>] [-ThrottleLimit <Int32>] -VMName <String[]> [<CommonParameters>]

New-PSSession [-VMId] <Guid[]> [-ConfigurationName <String>] -Credential <PSCredential> [-Name <String[]>] [-ThrottleLimit <Int32>] [<CommonParameters>]

New-PSSession [-ConfigurationName <String>] -ContainerId <String[]> [-Name <String[]>] [-RunAsAdministrator] [-ThrottleLimit <Int32>] [<CommonParameters>]

New-PSSession [[-Session] <PSSession[]>] [-EnableNetworkAccess] [-Name <String[]>] [-ThrottleLimit <Int32>] [<CommonParameters>]

New-PSSession [-HostName] <String[]> [-KeyFilePath <String>] [-Name <String[]>] [-Port <Int32>] [-SSHTransport {true}] [-UserName <String>] [<CommonParameters>]

New-PSSession [-Name <String[]>] -SSHConnection <Hashtable[]> [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-PSSessionConfigurationFile`, [
        new Parameter(`AliasDefinitions`, `Adds the specified aliases to sessions that use the session configuration. Enter a hash table with the following keys:

- Name. Name of the alias. This key is required. - Value. The command that the alias represents. This key is required. - Description. A text string that describes the alias. This key is optional. - Options. Alias options. This key is optional. The default value is None. The acceptable values for this parameter are: None, ReadOnly, Constant, Private, or AllScope.

For example: "@{Name="hlp";Value="Get-Help";Description="Gets help";Options="ReadOnly"}"`, `IDictionary[]`),
        new Parameter(`AssembliesToLoad`, `Specifies the assemblies to load into the sessions that use the session configuration.`, `String[]`),
        new Parameter(`Author`, `Specifies the author of the session configuration or the configuration file. The default is the current user. The value of this parameter is visible in the session configuration file, but it is not a property of the session configuration object.`, `String`),
        new Parameter(`CompanyName`, `Specifies the company that created the session configuration or the configuration file. The default value is Unknown. The value of this parameter is visible in the session configuration file, but it is not a property of the session configuration object.`, `String`),
        new Parameter(`Copyright`, `Specifies a copyright the session configuration file. The value of this parameter is visible in the session configuration file, but it is not a property of the session configuration object.

If you omit this parameter, New-PSSessionConfigurationFile generates a copyright statement by using the value of the Author parameter.`, `String`),
        new Parameter(`Description`, `Specifies a description of the session configuration or the session configuration file. The value of this parameter is visible in the session configuration file, but it is not a property of the session configuration object.`, `String`),
        new Parameter(`EnvironmentVariables`, `Adds environment variables to the session. Enter a hash table in which the keys are the environment variable names and the values are the environment variable values.

For example: "EnvironmentVariables=@{TestShare="\\Server01\TestShare"}"`, `IDictionary`),
        new Parameter(`ExecutionPolicy`, `Specifies the execution policy of sessions that use the session configuration. If you omit this parameter, the value of the ExecutionPolicy key in the session configuration file is Restricted. For information about execution policies in Windows PowerShell, see about_Execution_Policies (http://go.microsoft.com/fwlink/?LinkID=135170).`, `ExecutionPolicy`),
        new Parameter(`FormatsToProcess`, `Specifies the formatting files (.ps1xml) that run in sessions that use the session configuration. The value of this parameter must be a full or absolute path of the formatting files.`, `String[]`),
        new Parameter(`FunctionDefinitions`, `Adds the specified functions to sessions that use the session configuration. Enter a hash table with the following keys:

- Name. Name of the function. This key is required. - ScriptBlock. Function body. Enter a script block. This key is required. - Options. Function options. This key is optional. The default value is None. The acceptable values for this parameter are: None, ReadOnly, Constant, Private, or AllScope.

For example: "@{Name="Get-PowerShellProcess";ScriptBlock={Get-Process PowerShell};Options="AllScope"}"`, `IDictionary[]`),
        new Parameter(`Guid`, `Specifies a unique identifier for the session configuration file. If you omit this parameter, New-PSSessionConfigurationFile generates a GUID for the file.To create a new GUID in Windows PowerShell, type ""[guid]::NewGuid()"".`, `Guid`),
        new Parameter(`LanguageMode`, `Determines which elements of the Windows PowerShell language are permitted in sessions that use this session configuration. You can use this parameter to restrict the commands that particular users can run on the computer.

The acceptable values for this parameter are:

- FullLanguage. All language elements are permitted. - ConstrainedLanguage. Commands that contain scripts to be evaluated are not allowed. The ConstrainedLanguage mode restricts user access to Microsoft .NET Framework types, objects, or methods. - NoLanguage. Users may run cmdlets and functions, but are not permitted to use any language elements, such as script blocks, variables, or operators. - RestrictedLanguage. Users may run cmdlets and functions, but are not permitted to use script blocks or variables except for the following permitted variables: $PSCulture, $PSUICulture, $True, $False, and $Null. Users may use only the basic comparison operators (-eq, -gt, -lt). Assignment statements, property references, and method calls are not permitted.

The default value of the LanguageMode parameter depends on the value of the SessionType parameter.

- Empty. NoLanguage

- RestrictedRemoteServer. NoLanguage

- Default. FullLanguage`, `PSLanguageMode`),
        new Parameter(`ModulesToImport`, `Specifies the modules and snap-ins that are automatically imported into sessions that use the session configuration.

By default, only the Microsoft.PowerShell.Core snap-in is imported into remote sessions, but unless the cmdlets are excluded, users can use the Import-Module and Add-PSSnapin cmdlets to add modules and snap-ins to the session.

Each module or snap-in in the value of this parameter can be represented by a string or as a hash table. A module string consists only of the name of the module or snap-in. A module hash table can include ModuleName , ModuleVersion , and GUID keys. Only the ModuleName key is required.

For example, the following value consists of a string and a hash table. Any combination of strings and hash tables, in any order, is valid.

""TroubleshootingPack", @{ModuleName="PSDiagnostics"; ModuleVersion="1.0.0.0";GUID="c61d6278-02a3-4618-ae37-a524d40a7f44"},"

The value of the ModulesToImport parameter of the Register-PSSessionConfiguration cmdlet takes precedence over the value of the ModulesToImport key in the session configuration file.`, `Object[]`),
        new Parameter(`Path`, `Specifies the path and file name of the session configuration file. The file must have a .pssc file name extension.`, `String`),
        new Parameter(`PowerShellVersion`, `Specifies the version of the Windows PowerShell engine in sessions that use the session configuration. The acceptable values for this parameter are: 2.0 and 3.0. If you omit this parameter, the PowerShellVersion key is commented-out and newest version of Windows PowerShell runs in the session.

The value of the PSVersion parameter of the Register-PSSessionConfiguration cmdlet takes precedence over the value of the PowerShellVersion key in the session configuration file.`, `Version`),
        new Parameter(`SchemaVersion`, `Specifies the version of the session configuration file schema. The default value is "1.0.0.0".`, `Version`),
        new Parameter(`ScriptsToProcess`, `Adds the specified scripts to sessions that use the session configuration. Enter the path and file names of the scripts. The value of this parameter must be a full or absolute path of script file names.`, `String[]`),
        new Parameter(`SessionType`, `Specifies the type of session that is created by using the session configuration. The default value is Default. The acceptable values for this parameter are:

- Empty. No modules or snap-ins are added to session by default. Use the parameters of this cmdlet to add modules, functions, scripts, and other features to the session. This option is designed for you to create custom sessions by adding selected command. If you do not add commands to an empty session, the session is limited to expressions and might not be usable. - Default. Adds the Microsoft.PowerShell.Core snap-in to the session. This snap-in includes the Import-Module and Add-PSSnapin cmdlets that users can use to import other modules and snap-ins unless you explicitly prohibit the use of the cmdlets. - RestrictedRemoteServer. Includes only the following proxy functions:  Exit-PSSession, Get-Command, Get-FormatData, Get-Help, Measure-Object, Out-Default, and Select-Object. Use the parameters of this cmdlet to add modules, functions, scripts, and other features to the session.`, `SessionType`),
        new Parameter(`TypesToProcess`, `Adds the specified type files (.ps1xml) to sessions that use the session configuration. Enter the type file names. The value of this parameter must be a full or absolute path of type file names.`, `String[]`),
        new Parameter(`VariableDefinitions`, `Adds the specified variables to sessions that use the session configuration. Enter a hash table with the following keys:

- Name. Name of the variable. This key is required. - Value. Variable value. This key is required. - Options. Variable options. This key is optional. The default value is None. The acceptable values for this parameter are: None, ReadOnly, Constant, Private, or AllScope.

For example: "@{Name="WarningPreference";Value="SilentlyContinue";Options="AllScope"}"`, `Object`),
        new Parameter(`VisibleAliases`, `Limits the aliases in the session to those specified in the value of this parameter, plus any aliases that you define in the AliasDefinition parameter. Wildcard characters are supported. By default, all aliases that are defined by the Windows PowerShell engine and all aliases that modules export are visible in the session.

For example: "VisibleAliases="gcm", "gp""

When any Visible parameter is included in the session configuration file, Windows PowerShell removes the Import-Module cmdlet and its ipmo alias from the session.`, `String[]`),
        new Parameter(`VisibleCmdlets`, `Limits the cmdlets in the session to those specified in the value of this parameter. Wildcard characters and Module Qualified Names are supported.

By default, all cmdlets that modules in the session export are visible in the session. Use the SessionType and ModulesToImport parameters to determine which modules and snap-ins are imported into the session. If no modules in ModulesToImport expose the cmdlet, the appropriate module will attempt to be autoloaded.

When any Visible parameter is included in the session configuration file, Windows PowerShell removes the Import-Module cmdlet and its ipmo alias from the session.`, `Object[]`),
        new Parameter(`VisibleFunctions`, `Limits the functions in the session to those specified in the value of this parameter, plus any functions that you define in the FunctionDefinition parameter. Wildcard characters are supported.

By default, all functions that modules in the session export are visible in the session. Use the SessionType and ModulesToImport parameters to determine which modules and snap-ins are imported into the session.

When any Visible parameter is included in the session configuration file, Windows PowerShell removes the Import-Module cmdlet and its ipmo alias from the session.`, `Object[]`),
        new Parameter(`VisibleProviders`, `Limits the Windows PowerShell providers in the session to those specified in the value of this parameter. Wildcard characters are supported.

By default, all providers that modules in the session export are visible in the session. Use the SessionType and ModulesToImport parameters to determine which modules and snap-ins are imported into the session.

When any Visible parameter is included in the session configuration file, Windows PowerShell removes the Import-Module cmdlet and its ipmo alias from the session.`, `String[]`),
        new Parameter(`Full`, `Indicates that this operation includes all possible configuration properties in the session configuration file.`, `SwitchParameter`),
        new Parameter(`GroupManagedServiceAccount`, `Configures sessions using this session configuration to run under the context of the specified Group Managed Service Account. The machine where this session configuration is registered must have permission to request the gMSA password in order for sessions to be created successfully. This field cannot be used in conjunction with the "-RunAsVirtualAccount" parameter.`, `String`),
        new Parameter(`MountUserDrive`, `Configures sessions that use this session configuration to expose the "User:" PSDrive. User drives are unique for each connecting user and allow users to copy data to/from PowerShell endpoints even if the File System provider is not exposed. User drive roots are created under "$env:LOCALAPPDATA\Microsoft\Windows\PowerShell\DriveRoots".

Contents in the user drive persist across user sessions and are not automatically removed. By default, users can only store up to 50MB of data in the user drive. This can be customized with the "-UserDriveMaximumSize" parameter.`, `SwitchParameter`),
        new Parameter(`RequiredGroups`, `Specifies conditional access rules for users connecting to sessions that use this session configuration.

Enter a hashtable to compose your list of rules using only 1 key per hashtable, 'And' or 'Or', and set the value to an array of security group names or additional hashtables.

Example requiring connecting users to be members of a single group: "@{ And = 'MyRequiredGroup' }"

Example requiring users to belong to group A, or both groups B and C, to access the endpoint: "@{ Or = 'GroupA', @{ And = 'GroupB', 'GroupC' } }"`, `IDictionary`),
        new Parameter(`RoleDefinitions`, `Specifies the mapping between security groups (or users) and role capabilities. Users will be granted access to all role capabilities which apply to their group membership at the time the session is created.

Enter a hash table in which the keys are the name of the security group and the values are hash tables that contain a list of role capabilities that should be made available to the security group.

For example: "@{'Contoso\Level 2 Helpdesk Users' = @{ RoleCapabilities = 'Maintenance', 'ADHelpDesk' }}"`, `IDictionary`),
        new Parameter(`RunAsVirtualAccount`, `Configures sessions using this session configuration to be run as the computer's (virtual) administrator account. This field cannot be used in conjunction with the "-GroupManagedServiceAccount" parameter.`, `SwitchParameter`),
        new Parameter(`RunAsVirtualAccountGroups`, `Specifies the security groups to be associated with the virtual account when a session that uses the session configuration is run as a virtual account. If omitted, the virtual account belongs to Domain Admins on domain controllers and Administrators on all other computers.`, `String[]`),
        new Parameter(`TranscriptDirectory`, `Specifies the directory to place session transcripts for sessions using this session configuration.`, `String`),
        new Parameter(`UserDriveMaximumSize`, `Specifies the maximum size for user drives exposed in sessions that use this session configuration. When omitted, the default size of each User: drive root is 50MB.

This parameter should be used in conjunction with "-MountUserDrive".`, `Int64`),
        new Parameter(`VisibleExternalCommands`, `Limits the external binaries, scripts, and commands that can be executed in the session to those specified in the value of this parameter. Wildcard characters are supported.

By default, no external commands are visible in the session.

When any Visible parameter is included in the session configuration file, Windows PowerShell, removes the Import-Module cmdlet and its ipmo alias from the session.`, `String[]`),
    ], `Creates a file that defines a session configuration.`, `New-PSSessionConfigurationFile [-Path] <String> [-AliasDefinitions <IDictionary[]>] [-AssembliesToLoad <String[]>] [-Author <String>] [-CompanyName <String>] [-Copyright <String>] [-Description <String>] [-EnvironmentVariables <IDictionary>] [-ExecutionPolicy {Unrestricted | RemoteSigned | AllSigned | Restricted | Default | Bypass | Undefined}] [-FormatsToProcess <String[]>] [-FunctionDefinitions <IDictionary[]>] [-Guid <Guid>] [-LanguageMode {FullLanguage | RestrictedLanguage | NoLanguage | ConstrainedLanguage}] [-ModulesToImport <Object[]>] [-PowerShellVersion <Version>] [-SchemaVersion <Version>] [-ScriptsToProcess <String[]>] [-SessionType {Empty | RestrictedRemoteServer | Default}] [-TypesToProcess <String[]>] [-VariableDefinitions <Object>] [-VisibleAliases <String[]>] [-VisibleCmdlets <Object[]>] [-VisibleFunctions <Object[]>] [-VisibleProviders <String[]>] [-Full] [-GroupManagedServiceAccount <String>] [-MountUserDrive] [-RequiredGroups <IDictionary>] [-RoleDefinitions <IDictionary>] [-RunAsVirtualAccount] [-RunAsVirtualAccountGroups <String[]>] [-TranscriptDirectory <String>] [-UserDriveMaximumSize <Int64>] [-VisibleExternalCommands <String[]>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-PSSessionOption`, [
        new Parameter(`ApplicationArguments`, `Specifies a primitive dictionary that is sent to the remote session. Commands and scripts in the remote session, including startup scripts in the session configuration, can find this dictionary in the ApplicationArguments property of the $PSSenderInfo automatic variable. You can use this parameter to send data to the remote session.

A primitive dictionary is like a hash table, but it contains keys that are case-insensitive strings and values that can be serialized and deserialized during PowerShell remoting handshakes. If you enter a hash table for the value of this parameter, PowerShell converts it to a primitive dictionary.

For more information, see about_Hash_Tables (http://go.microsoft.com/fwlink/?LinkID=135175), about_Session_Configurations (About/about_Session_Configurations.md), and about_Automatic_Variables (http://go.microsoft.com/fwlink/?LinkID=113212).`, `PSPrimitiveDictionary`),
        new Parameter(`CancelTimeout`, `Determines how long PowerShell waits for a cancel operation (CTRL + C) to finish before ending it. Enter a value in milliseconds.

The default value is 60000 (one minute). A value of 0 (zero) means no time-out; the command continues indefinitely.`, `Int32`),
        new Parameter(`Culture`, `Specifies the culture to use for the session. Enter a culture name in <languagecode2>-<country/regioncode2> format, such as ja-jP, a variable that contains a CultureInfo object, or a command that gets a CultureInfo object, such as Get-Culture.

The default value is $Null, and the culture that is set in the operating system is used in the session.`, `CultureInfo`),
        new Parameter(`IdleTimeout`, `Determines how long the session stays open if the remote computer does not receive any communication from the local computer. This includes the heartbeat signal. When the interval expires, the session closes.

The idle time-out value is of significant importance if you intend to disconnect and reconnect to a session. You can reconnect only if the session has not timed out.

Enter a value in milliseconds. The minimum value is 60000 (1 minute). The maximum is the value of the MaxIdleTimeoutms property of the session configuration. The default value, -1, does not set an idle time-out.

The session uses the idle time-out that is set in the session options, if any. If none is set (-1), the session uses the value of the IdleTimeoutMs property of the session configuration or the WSMan shell time-out value ("WSMan:\<ComputerName>\Shell\IdleTimeout"), whichever is shortest.

If the idle timeout set in the session options exceeds the value of the MaxIdleTimeoutMs property of the session configuration, the command to create a session fails.

The IdleTimeoutMs value of the default Microsoft.PowerShell session configuration is 7200000 milliseconds (2 hours). Its MaxIdleTimeoutMs value is 2147483647 milliseconds (>24 days). The default value of the WSMan shell idle time-out ("WSMan:\<ComputerName>\Shell\IdleTimeout") is 7200000 milliseconds (2 hours).

The idle time-out value of a session can also be changed when disconnecting from a session or reconnecting to a session. For more information, see Disconnect-PSSession and Connect-PSSession.

In Windows PowerShell 2.0, the default value of the IdleTimeout parameter is 240000 (4 minutes).`, `Int32`),
        new Parameter(`IncludePortInSPN`, `Includes the port number in the Service Principal Name (SPN) used for Kerberos authentication, for example, "HTTP/<ComputerName>:5985". This option allows a client that uses a non-default SPN to authenticate against a remote computer that uses Kerberos authentication.

The option is designed for enterprises where multiple services that support Kerberos authentication are running under different user accounts. For example, an IIS application that allows for Kerberos authentication can require the default SPN to be registered to a user account that differs from the computer account. In such cases, PowerShell remoting cannot use Kerberos to authenticate because it requires an SPN that is registered to the computer account. To resolve this problem, administrators can create different SPNs, such as by using Setspn.exe, that are registered to different user accounts and can distinguish between them by including the port number in the SPN.

For more information about Setspn.exe, see Setspn Overview (https://go.microsoft.com/fwlink/?LinkID=189413).

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`MaxConnectionRetryCount`, `Specifies the number of times that PowerShell attempts to make a connection to a target machine if the current attempt fails due to network issues. The default value is 5.

This parameter was added for PowerShell version 5.0.`, `Int32`),
        new Parameter(`MaximumReceivedDataSizePerCommand`, `Specifies the maximum number of bytes that the local computer can receive from the remote computer in a single command. Enter a value in bytes. By default, there is no data size limit.

This option is designed to protect the resources on the client computer.`, `Int32`),
        new Parameter(`MaximumReceivedObjectSize`, `Specifies the maximum size of an object that the local computer can receive from the remote computer. This option is designed to protect the resources on the client computer. Enter a value in bytes.

In Windows PowerShell 2.0, if you omit this parameter, there is no object size limit. Beginning in Windows PowerShell 3.0, if you omit this parameter, the default value is 200 MB.`, `Int32`),
        new Parameter(`MaximumRedirection`, `Determines how many times PowerShell redirects a connection to an alternate Uniform Resource Identifier (URI) before the connection fails. The default value is 5. A value of 0 (zero) prevents all redirection.

This option is used in the session only when the AllowRedirection parameter is used in the command that creates the session.`, `Int32`),
        new Parameter(`NoCompression`, `Turns off packet compression in the session. Compression uses more processor cycles, but it makes transmission faster.`, `SwitchParameter`),
        new Parameter(`NoEncryption`, `Turns off data encryption.`, `SwitchParameter`),
        new Parameter(`NoMachineProfile`, `Prevents loading the user's Windows user profile. As a result, the session might be created faster, but user-specific registry settings, items such as environment variables, and certificates are not available in the session.`, `SwitchParameter`),
        new Parameter(`OpenTimeout`, `Determines how long the client computer waits for the session connection to be established. When the interval expires, the command to establish the connection fails. Enter a value in milliseconds.

The default value is 180000 (3 minutes). A value of 0 (zero) means no time-out; the command continues indefinitely.`, `Int32`),
        new Parameter(`OperationTimeout`, `Determines the maximum time that any operation in the session can run. When the interval expires, the operation fails. Enter a value in milliseconds.

The default value is 180000 (3 minutes). A value of 0 (zero) means no time-out; the operation continues indefinitely.`, `Int32`),
        new Parameter(`OutputBufferingMode`, `Determines how command output is managed in disconnected sessions when the output buffer becomes full.

If the output buffering mode is not set in the session or in the session configuration, the default value is Block . Users can also change the output buffering mode when disconnecting the session.

If you omit this parameter, the value of the OutputBufferingMode of the session option object is None. A value of Block or Drop overrides the output buffering mode transport option set in the session configuration. The acceptable values for this parameter are:

- Block. When the output buffer is full, execution is suspended until the buffer is clear. - Drop. When the output buffer is full, execution continues. As new output is saved, the oldest output is discarded. - None. No output buffering mode is specified.

For more information about the output buffering mode transport option, see New-PSTransportOption.

This parameter was introduced in Windows PowerShell 3.0.`, `OutputBufferingMode`),
        new Parameter(`ProxyAccessType`, `Determines which mechanism is used to resolve the host name. The acceptable values for this parameter are:

- IEConfig

- WinHttpConfig

- AutoDetect

- NoProxyServer

- None



The default value is None.

For information about the values of this parameter, see ProxyAccessType Enumeration (https://msdn.microsoft.com/library/system.management.automation.remoting.proxyaccesstype)in the MSDN library.`, `ProxyAccessType`),
        new Parameter(`ProxyAuthentication`, `Specifies the authentication method that is used for proxy resolution. The acceptable values for this parameter are: Basic,  Digest, and Negotiate. The default value is Negotiate.

For more information about the values of this parameter, see AuthenticationMechanism Enumeration (https://msdn.microsoft.com/library/system.management.automation.runspaces.authenticationmechanism)in the MSDN library.`, `AuthenticationMechanism`),
        new Parameter(`ProxyCredential`, `Specifies the credentials to use for proxy authentication. Enter a variable that contains a PSCredential object or a command that gets a PSCredential object, such as a Get-Credential command. If this option is not set, no credentials are specified.`, `PSCredential`),
        new Parameter(`SkipCACheck`, `Specifies that when it connects over HTTPS, the client does not validate that the server certificate is signed by a trusted certification authority (CA).

Use this option only when the remote computer is trusted by using another mechanism, such as when the remote computer is part of a network that is physically secure and isolated or when the remote computer is listed as a trusted host in a WinRM configuration.`, `SwitchParameter`),
        new Parameter(`SkipCNCheck`, `Specifies that the certificate common name (CN) of the server does not have to match the host name of the server. This option is used only in remote operations that use the HTTPS protocol.

Use this option only for trusted computers.`, `SwitchParameter`),
        new Parameter(`SkipRevocationCheck`, `Does not validate the revocation status of the server certificate.`, `SwitchParameter`),
        new Parameter(`UICulture`, `Specifies the UI culture to use for the session.

Enter a culture name in <languagecode2>-<country/regioncode2> format, such as ja-jP, a variable that contains a CultureInfo object, or a command that gets a CultureInfo object, such as Get-Culture .

The default value is $Null, and the UI culture that is set in the operating system when the session is created is used in the session.`, `CultureInfo`),
        new Parameter(`UseUTF16`, `Indicates that this cmdlet encodes the request in UTF16 format instead of UTF8 format.`, `SwitchParameter`),
    ], `Creates an object that contains advanced options for a PSSession.`, `New-PSSessionOption [-ApplicationArguments <PSPrimitiveDictionary>] [-CancelTimeout <Int32>] [-Culture <CultureInfo>] [-IdleTimeout <Int32>] [-IncludePortInSPN] [-MaxConnectionRetryCount <Int32>] [-MaximumReceivedDataSizePerCommand <Int32>] [-MaximumReceivedObjectSize <Int32>] [-MaximumRedirection <Int32>] [-NoCompression] [-NoEncryption] [-NoMachineProfile] [-OpenTimeout <Int32>] [-OperationTimeout <Int32>] [-OutputBufferingMode {None | Drop | Block}] [-ProxyAccessType {None | IEConfig | WinHttpConfig | AutoDetect | NoProxyServer}] [-ProxyAuthentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-ProxyCredential <PSCredential>] [-SkipCACheck] [-SkipCNCheck] [-SkipRevocationCheck] [-UICulture <CultureInfo>] [-UseUTF16] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-PSTransportOption`, [
        new Parameter(`IdleTimeoutSec`, `Determines how long each session stays open if the remote computer does not receive any communication from the local computer. This includes the heartbeat signal. When the interval expires, the session closes.

The idle time-out value is of significant importance when the user intends to disconnect and reconnect to a session. The user can reconnect only if the session has not timed out.

The IdleTimeoutSec parameter corresponds to the IdleTimeoutMs property of a session configuration.

Enter a value in seconds. The default value is 7200 (2 hours). The minimum value is 60 (1 minute). The maximum is the value of the IdleTimeout property of Shell objects in the WSMan configuration ("WSMan:\<ComputerName>\Shell\IdleTimeout"). The default value is 7200000 milliseconds (2 hours).

If an idle time-out value is set in the session options and in the session configuration, value set in the session options takes precedence, but it cannot exceed the value of the MaxIdleTimeoutMs property of the session configuration. To set the value of the MaxIdleTimeoutMs property, use the MaxIdleTimeoutSec parameter.`, `Int32`),
        new Parameter(`MaxConcurrentCommandsPerSession`, `Limits the number of commands that can run at the same time in each session to the specified value. The default value is 1000.

The MaxConcurrentCommandsPerSession parameter corresponds to the MaxConcurrentCommandsPerShell property of a session configuration.`, `Int32`),
        new Parameter(`MaxConcurrentUsers`, `Limits the number of users who can run commands at the same time in each session to the specified value. The default value is 5.`, `Int32`),
        new Parameter(`MaxIdleTimeoutSec`, `Limits the idle time-out set for each session to the specified value. The default value is [Int]::MaxValue (~25 days).

The idle time-out value is of significant importance when the user intends to disconnect and reconnect to a session. The user can reconnect only if the session has not timed out.

The MaxIdleTimeoutSec parameter corresponds to the MaxIdleTimeoutMs property of a session configuration.`, `Int32`),
        new Parameter(`MaxMemoryPerSessionMB`, `Limits the memory used by each session to the specified value. Enter a value in megabytes. The default value is 1024 megabytes (1 GB).

The MaxMemoryPerSessionMB parameter corresponds to the MaxMemoryPerShellMB property of a session configuration.`, `Int32`),
        new Parameter(`MaxProcessesPerSession`, `Limits the number of processes running in each session to the specified value. The default value is 15.

The MaxProcessesPerSession parameter corresponds to the MaxProcessesPerShell property of a session configuration.`, `Int32`),
        new Parameter(`MaxSessions`, `Limits the number of sessions that use the session configuration. The default value is 25.

The MaxSessions parameter corresponds to the MaxShells property of a session configuration.`, `Int32`),
        new Parameter(`MaxSessionsPerUser`, `Limits the number of sessions that use the session configuration and run with the credentials of a given user to the specified value. The default value is 25.

When you specify this value, consider that many users might be using the credentials of a run as user.

The MaxSessionsPerUser parameter corresponds to the MaxShellsPerUser property of a session configuration.`, `Int32`),
        new Parameter(`OutputBufferingMode`, `Determines how command output is managed in disconnected sessions when the output buffer becomes full. The acceptable values for this parameter are:

- Block. When the output buffer is full, execution is suspended until the buffer is clear. - Drop. When the output buffer is full, execution continues. As new output is saved, the oldest output is discarded. - None. No output buffering mode is specified.

The default value of the OutputBufferingMode property of sessions is Block.`, `OutputBufferingMode`),
        new Parameter(`ProcessIdleTimeoutSec`, `Limits the time-out for each host process to the specified value. The default value, 0, means that there is no time-out value for the process.

Other session configurations have per-process time-out values. For example, the Microsoft.PowerShell.Workflow session configuration has a per-process time-out value of 28800 seconds (8 hours).`, `Int32`),
    ], `Creates an object that contains advanced options for a session configuration.`, `New-PSTransportOption [-IdleTimeoutSec <Int32>] [-MaxConcurrentCommandsPerSession <Int32>] [-MaxConcurrentUsers <Int32>] [-MaxIdleTimeoutSec <Int32>] [-MaxMemoryPerSessionMB <Int32>] [-MaxProcessesPerSession <Int32>] [-MaxSessions <Int32>] [-MaxSessionsPerUser <Int32>] [-OutputBufferingMode {None | Drop | Block}] [-ProcessIdleTimeoutSec <Int32>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-Service`, [
        new Parameter(`BinaryPathName`, `Specifies the path of the executable file for the service. This parameter is required.`, `String`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.`, `PSCredential`),
        new Parameter(`DependsOn`, `Specifies the names of other services upon which the new service depends. To enter multiple service names, use a comma to separate the names.`, `String[]`),
        new Parameter(`Description`, `Specifies a description of the service.`, `String`),
        new Parameter(`DisplayName`, `Specifies a display name for the service.`, `String`),
        new Parameter(`Name`, `Specifies the name of the service. This parameter is required.`, `String`),
        new Parameter(`StartupType`, `Sets the startup type of the service. The acceptable values for this parameter are:

- Manual. The service is started only manually, by a user, using the Service Control Manager, or by an application. - Automatic. The service is started or was started by the operating system, at system start-up. If an automatically started service depends on a manually started service, the manually started service is also started automatically at system startup. - Disabled. The service is disabled and cannot be started by a user or application.

The default value is Automatic.`, `ServiceStartMode`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Creates a new Windows service.`, `New-Service [-Name] <String> [-BinaryPathName] <String> [-Credential <PSCredential>] [-DependsOn <String[]>] [-Description <String>] [-DisplayName <String>] [-StartupType {Automatic | Manual | Disabled}] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-TemporaryFile`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-TimeSpan`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-Variable`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-WinEvent`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-WSManInstance`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`New-WSManSessionOption`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Out-Default`, [
        new Parameter(`InputObject`, `Accepts input to the cmdlet.`, `PSObject`),
        new Parameter(`Transcript`, ``, `SwitchParameter`),
    ], `Sends the output to the default formatter and to the default output cmdlet.`, `Out-Default [-InputObject <PSObject>] [-Transcript] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Out-File`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Out-Host`, [
        new Parameter(`InputObject`, `Specifies the objects that are written to the console. Enter a variable that contains the objects, or type a command or expression that gets the objects.`, `PSObject`),
        new Parameter(`Paging`, `Indicates that this cmdlet displays one page of output at a time, and waits for user input before it displays the remaining pages, much like the traditional more command. By default, all of the output is displayed on a single page. The page size is determined by the characteristics of the host.`, `SwitchParameter`),
    ], `Sends output to the command line.`, `Out-Host [-InputObject <PSObject>] [-Paging] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Out-Null`, [
        new Parameter(`InputObject`, `Specifies the object to be sent to NULL (removed from pipeline). Enter a variable that contains the objects, or type a command or expression that gets the objects.`, `PSObject`),
    ], `Hides the output instead of sending it down the pipeline or displaying it.`, `Out-Null [-InputObject <PSObject>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Out-String`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Pop-Location`, [
        new Parameter(`PassThru`, `Passes an object that represents the location to the pipeline. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`StackName`, `Specifies the location stack from which the location is popped. Enter a location stack name.

Without this parameter, Pop-Location pops a location from the current location stack. By default, the current location stack is the unnamed default location stack that PowerShell creates. To make a location stack the current location stack, use the StackName parameter of Set-Location . Pop-Location cannot pop a location from the unnamed default stack unless it is the current location stack.`, `String`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Changes the current location to the location most recently pushed onto the stack.`, `Pop-Location [-PassThru] [-StackName <String>] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Protect-CmsMessage`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Push-Location`, [
        new Parameter(`LiteralPath`, `Specifies the path of the new location. Unlike the Path parameter, the value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String`),
        new Parameter(`PassThru`, `Passes an object that represents the location to the pipeline. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies the path of the new location. This cmdlet your location to the location specified by this path after it adds, or pushes, the current location onto the top of the stack. Enter a path of any location whose provider supports this cmdlet. Wildcard characters are permitted.`, `String`),
        new Parameter(`StackName`, `Specifies the location stack to which the current location is added. Enter a location stack name. If the stack does not exist, Push-Location creates it.

Without this parameter, Push-Location adds the location to the current location stack. By default, the current location stack is the unnamed default location stack that PowerShell creates. To make a location stack the current location stack, use the StackName parameter of the Set-Location cmdlet. For more information about location stacks, see the Notes. Push-Location cannot add a location to the unnamed default stack unless it is the current location stack.`, `String`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Adds the current location to the top of a location stack.`, `Push-Location [-LiteralPath <String>] [-PassThru] [-StackName <String>] [-UseTransaction] [<CommonParameters>]

Push-Location [[-Path] <String>] [-PassThru] [-StackName <String>] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Read-Host`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Receive-Job`, [
        new Parameter(`AutoRemoveJob`, `Indicates that this cmdlet deletes the job after it returns the job results. If the job has more results, the job is still deleted, but Receive-Job displays a message.

This parameter works only on custom job types. It is designed for instances of job types that save the job or the type outside of the session, such as instances of scheduled jobs.

This parameter cannot be used without the Wait parameter.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`ComputerName`, `Specifies an array of names of computers. This cmdlet gets the results of jobs that were run on the specified computers. Enter the computer names. Wildcard characters are supported. The default is all jobs in the current session.

This parameter selects from among the job results that are stored on the local computer. It does not get data from remote computers. To get job results that are stored on remote computers, use the Invoke-Command cmdlet to run a Receive-Job command remotely.`, `String[]`),
        new Parameter(`Force`, `Indicates that this cmdlet continues waiting if jobs are in the Suspended or Disconnected state. By default, the Wait parameter of Receive-Job returns, or terminates the wait, when jobs are in one of the following states:  Completed, Failed, Stopped, Suspended, or Disconnected.

The Force parameter is valid only when the Wait parameter is also used in the command.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`Id`, `Specifies an array of IDs. This cmdlet gets the results of jobs with the specified IDs. The default is all jobs in the current session.

The ID is an integer that uniquely identifies the job in the current session. It is easier to remember and type than the instance ID, but it is unique only in the current session. You can type one or more IDs separated by commas. To find the ID of a job, type "Get-Job" without parameters.`, `Int32[]`),
        new Parameter(`InstanceId`, `Specifies an array of instance IDs. This cmdlet gets the results of jobs with the specified instance IDs. The default is all jobs in the current session.

An instance ID is a GUID that uniquely identifies the job on the computer. To find the instance ID of a job, use the Get-Job cmdlet.`, `Guid[]`),
        new Parameter(`Job`, `Specifies the job for which results are being retrieved. This parameter is required in a Receive-Job command. Enter a variable that contains the job or a command that gets the job. You can also pipe a job object to Receive-Job .`, `Job[]`),
        new Parameter(`Keep`, `Indicates that this cmdlet saves the job results in the system, even after you have received them. By default, the job results are deleted when they are retrieved.

To delete the results, use Receive-Job to receive them again without specifying Keep parameter, close the session, or use the Remove-Job cmdlet to delete the job from the session.`, `SwitchParameter`),
        new Parameter(`Location`, `Specifies an array of locations. This cmdlet gets only the results of jobs in the specified locations. The default is all jobs in the current session.`, `String[]`),
        new Parameter(`Name`, `Specifies an array of friendly names. This cmdlet gets the results of jobs that have the specified names. Wildcard characters are supported. The default is all jobs in the current session.`, `String[]`),
        new Parameter(`NoRecurse`, `Indicates that this cmdlet gets results only from the specified job. By default, Receive-Job also gets the results of all child jobs of the specified job.`, `SwitchParameter`),
        new Parameter(`Session`, `Specifies an array of sessions. This cmdlet gets the results of jobs that were run in the specified PowerShell session ( PSSession ). Enter a variable that contains the PSSession or a command that gets the PSSession , such as a Get-PSSession command. The default is all jobs in the current session.`, `PSSession[]`),
        new Parameter(`Wait`, `Indicates that this cmdlet suppresses the command prompt until all job results are received. By default, Receive-Job immediately returns the available results.

By default, the Wait parameter waits until the job is in one of the following states: Completed, Failed, Stopped, Suspended, or Disconnected. To direct the Wait parameter to continue waiting if the job state is Suspended or Disconnected, use the Force parameter together with the Wait parameter.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`WriteEvents`, `Indicates that this cmdlet reports changes in the job state while it waits for the job to finish.

This parameter is valid only when the Wait parameter is used in the command and the Keep parameter is omitted.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`WriteJobInResults`, `Indicates that this cmdlet returns the job object followed by the results.

This parameter is valid only when the Wait parameter is used in the command and the Keep parameter is omitted.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
    ], `Gets the results of the PowerShell background jobs in the current session.`, `Receive-Job [-Job] <Job[]> [[-ComputerName] <String[]>] [-AutoRemoveJob] [-Force] [-Keep] [-NoRecurse] [-Wait] [-WriteEvents] [-WriteJobInResults] [<CommonParameters>]

Receive-Job [-Id] <Int32[]> [-AutoRemoveJob] [-Force] [-Keep] [-NoRecurse] [-Wait] [-WriteEvents] [-WriteJobInResults] [<CommonParameters>]

Receive-Job [-InstanceId] <Guid[]> [-AutoRemoveJob] [-Force] [-Keep] [-NoRecurse] [-Wait] [-WriteEvents] [-WriteJobInResults] [<CommonParameters>]

Receive-Job [-Job] <Job[]> [[-Location] <String[]>] [-AutoRemoveJob] [-Force] [-Keep] [-NoRecurse] [-Wait] [-WriteEvents] [-WriteJobInResults] [<CommonParameters>]

Receive-Job [-Job] <Job[]> [[-Session] <PSSession[]>] [-AutoRemoveJob] [-Force] [-Keep] [-NoRecurse] [-Wait] [-WriteEvents] [-WriteJobInResults] [<CommonParameters>]

Receive-Job [-Name] <String[]> [-AutoRemoveJob] [-Force] [-Keep] [-NoRecurse] [-Wait] [-WriteEvents] [-WriteJobInResults] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Receive-PSSession`, [
        new Parameter(`AllowRedirection`, `Indicates that this cmdlet allows redirection of this connection to an alternate Uniform Resource Identifier (URI).

When you use the ConnectionURI parameter, the remote destination can return an instruction to redirect to a different URI. By default, PowerShell does not redirect connections, but you can use this parameter to enable it to redirect the connection.

You can also limit the number of times the connection is redirected by changing the MaximumConnectionRedirectionCount session option value. Use the MaximumRedirection parameter of the New-PSSessionOption cmdlet or set the MaximumConnectionRedirectionCount property of the $PSSessionOption preference variable. The default value is 5.`, `SwitchParameter`),
        new Parameter(`ApplicationName`, `Specifies an application. This cmdlet connects only to sessions that use the specified application.

Enter the application name segment of the connection URI. For example, in the following connection URI, the application name is WSMan: "http://localhost:5985/WSMAN". The application name of a session is stored in the Runspace.ConnectionInfo.AppName property of the session.

The value of this parameter is used to select and filter sessions. It does not change the application that the session uses.`, `String`),
        new Parameter(`Authentication`, `Specifies the mechanism that is used to authenticate the credentials of the user in the command to reconnect to the disconnected session. The acceptable values for this parameter are:

- Default

- Basic

- Credssp

- Digest

- Kerberos

- Negotiate

- NegotiateWithImplicitCredential



The default value is Default.

For more information about the values of this parameter, see AuthenticationMechanism Enumeration (https://msdn.microsoft.com/library/system.management.automation.runspaces.authenticationmechanism)in the MSDN library.

CAUTION: Credential Security Support Provider (CredSSP) authentication, in which the user credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.`, `AuthenticationMechanism`),
        new Parameter(`CertificateThumbprint`, `Specifies the digital public key certificate (X509) of a user account that has permission to connect to the disconnected session. Enter the certificate thumbprint of the certificate.

Certificates are used in client certificate-based authentication. They can be mapped only to local user accounts. They do not work with domain accounts.

To get a certificate thumbprint, use a Get-Item or Get-ChildItem command in the PowerShell Cert: drive.`, `String`),
        new Parameter(`ComputerName`, `Specifies the computer on which the disconnected session is stored. Sessions are stored on the computer that is at the server-side, or receiving end of a connection. The default is the local computer.

Type the NetBIOS name, an IP address, or a fully qualified domain name of one computer. Wildcard characters are not permitted. To specify the local computer, type the computer name, localhost, or a dot (.)`, `String`),
        new Parameter(`ConfigurationName`, `Specifies the name of a session configuration. This cmdlet connects only to sessions that use the specified session configuration.

Enter a configuration name or the fully qualified resource URI for a session configuration. If you specify only the configuration name, the following schema URI is prepended: http://schemas.microsoft.com/powershell. The configuration name of a session is stored in the ConfigurationName property of the session.

The value of this parameter is used to select and filter sessions. It does not change the session configuration that the session uses.

For more information about session configurations, see about_Session_Configurations (About/about_Session_Configurations.md).`, `String`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`ConnectionUri`, `Specifies a URI that defines the connection endpoint that is used to reconnect to the disconnected session.

The URI must be fully qualified. The format of this string is as follows:

<Transport>://<ComputerName>:<Port>/<ApplicationName>

The default value is as follows:

"http://localhost:5985/WSMAN"

"http://localhost:5985/WSMAN"

If you do not specify a connection URI, you can use the UseSSL , ComputerName , Port , and ApplicationName parameters to specify the connection URI values.

Valid values for the Transport segment of the URI are HTTP and HTTPS. If you specify a connection URI with a Transport segment, but do not specify a port, the session is created with standards ports: 80 for HTTP and 443 for HTTPS. To use the default ports for Windows PowerShell remoting, specify port 5985 for HTTP or 5986 for HTTPS.

If the destination computer redirects the connection to a different URI, PowerShell prevents the redirection unless you use the AllowRedirection parameter in the command.`, `Uri`),
        new Parameter(`Credential`, `Specifies a user account that has permission to connect to the disconnected session. The default is the current user.

Type a user name, such as User01 or Domain01\User01. Or, enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.`, `PSCredential`),
        new Parameter(`Id`, `Specifies the ID of the disconnected session. The Id parameter works only when the disconnected session was previously connected to the current session.

This parameter is valid, but not effective, when the session is stored on the local computer, but was not connected to the current session.`, `Int32`),
        new Parameter(`InstanceId`, `Specifies the instance ID of the disconnected session.

The instance ID is a GUID that uniquely identifies a PSSession on a local or remote computer.

The instance ID is stored in the InstanceID property of the PSSession .`, `Guid`),
        new Parameter(`JobName`, `Specifies a friendly name for the job that Receive-PSSession returns. Receive-PSSession returns a job when the value of the OutTarget parameter is Job or the job that is running in the disconnected session was started in the current session.

If the job that is running in the disconnected session was started in the current session, PowerShell reuses the original job object in the session and ignores the value of the JobName parameter.

If the job that is running in the disconnected session was started in a different session, PowerShell creates a new job object. It uses a default name, but you can use this parameter to change the name.

If the default value or explicit value of the OutTarget parameter is not Job, the command succeeds, but the JobName parameter has no effect.`, `String`),
        new Parameter(`Name`, `Specifies the friendly name of the disconnected session.`, `String`),
        new Parameter(`OutTarget`, `Determines how the session results are returned. The acceptable values for this parameter are:

- Job. Returns the results asynchronously in a job object. You can use the JobName parameter to specify a name or new name for the job. - Host. Returns the results to the command line (synchronously). If the command is being resumed or the results consist of a large number of objects, the response might be delayed.

The default value of the OutTarget parameter is Host. However, if the command that is being received in disconnected session was started in the current session, the default value of the OutTarget parameter is the form in which the command was started. If the command was started as a job, it is returned as a job by default. Otherwise, it is returned to the host program by default.

Typically, the host program displays returned objects at the command line without delay, but this behavior can vary.`, `OutTarget`),
        new Parameter(`Port`, `Specifies the network port on the remote computer that is used to reconnect to the session. To connect to a remote computer, the remote computer must be listening on the port that the connection uses. The default ports are 5985, which is the WinRM port for HTTP, and 5986, which is the WinRM port for HTTPS.

Before using an alternate port, you must configure the WinRM listener on the remote computer to listen at that port. To configure the listener, type the following two commands at the PowerShell prompt:

"Remove-Item -Path WSMan:\Localhost\listener\listener* -Recurse"

"New-Item -Path WSMan:\Localhost\listener -Transport http -Address * -Port <port-number>"

Do not use the Port parameter unless you must. The port that is set in the command applies to all computers or sessions on which the command runs. An alternate port setting might prevent the command from running on all computers.`, `Int32`),
        new Parameter(`Session`, `Specifies the disconnected session. Enter a variable that contains the PSSession or a command that creates or gets the PSSession , such as a Get-PSSession command.`, `PSSession`),
        new Parameter(`SessionOption`, `Specifies advanced options for the session. Enter a SessionOption object, such as one that you create by using the New-PSSessionOption cmdlet, or a hash table in which the keys are session option names and the values are session option values.

The default values for the options are determined by the value of the $PSSessionOption preference variable, if it is set. Otherwise, the default values are established by options set in the session configuration.

The session option values take precedence over default values for sessions set in the $PSSessionOption preference variable and in the session configuration. However, they do not take precedence over maximum values, quotas or limits set in the session configuration.

For a description of the session options that includes the default values, see New-PSSessionOption. For information about the $PSSessionOption preference variable, see about_Preference_Variables (About/about_Preference_Variables.md). For more information about session configurations, see about_Session_Configurations (About/about_Session_Configurations.md).`, `PSSessionOption`),
        new Parameter(`UseSSL`, `Indicates that this cmdlet uses the Secure Sockets Layer (SSL) protocol to connect to the disconnected session. By default, SSL is not used.

WS-Management encrypts all PowerShell content transmitted over the network. UseSSL is an additional protection that sends the data across an HTTPS connection instead of an HTTP connection.

If you use this parameter, but SSL is not available on the port that is used for the command, the command fails.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Gets results of commands in disconnected sessions.`, `Receive-PSSession [-ConnectionUri] <Uri> [-AllowRedirection] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Confirm] [-Credential <PSCredential>] [-JobName <String>] -Name <String> [-OutTarget {Default | Host | Job}] [-SessionOption <PSSessionOption>] [-WhatIf] [<CommonParameters>]

Receive-PSSession [-ConnectionUri] <Uri> [-AllowRedirection] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Confirm] [-Credential <PSCredential>] -InstanceId <Guid> [-JobName <String>] [-OutTarget {Default | Host | Job}] [-SessionOption <PSSessionOption>] [-WhatIf] [<CommonParameters>]

Receive-PSSession [-ComputerName] <String> [-ApplicationName <String>] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Confirm] [-Credential <PSCredential>] [-JobName <String>] -Name <String> [-OutTarget {Default | Host | Job}] [-Port <Int32>] [-SessionOption <PSSessionOption>] [-UseSSL] [-WhatIf] [<CommonParameters>]

Receive-PSSession [-ComputerName] <String> [-ApplicationName <String>] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-CertificateThumbprint <String>] [-ConfigurationName <String>] [-Confirm] [-Credential <PSCredential>] -InstanceId <Guid> [-JobName <String>] [-OutTarget {Default | Host | Job}] [-Port <Int32>] [-SessionOption <PSSessionOption>] [-UseSSL] [-WhatIf] [<CommonParameters>]

Receive-PSSession [-Id] <Int32> [-Confirm] [-JobName <String>] [-OutTarget {Default | Host | Job}] [-WhatIf] [<CommonParameters>]

Receive-PSSession [-Confirm] -InstanceId <Guid> [-JobName <String>] [-OutTarget {Default | Host | Job}] [-WhatIf] [<CommonParameters>]

Receive-PSSession [-Confirm] [-JobName <String>] -Name <String> [-OutTarget {Default | Host | Job}] [-WhatIf] [<CommonParameters>]

Receive-PSSession [-Session] <PSSession> [-Confirm] [-JobName <String>] [-OutTarget {Default | Host | Job}] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Register-ArgumentCompleter`, [
        new Parameter(`CommandName`, `Specifies the name of the command as an array.`, `String[]`),
        new Parameter(`Native`, `Indicates that the argument completer is for a native command where PowerShell cannot complete parameter names.`, `SwitchParameter`),
        new Parameter(`ParameterName`, `Specifies the name of the parameter whose argument is being completed.`, `String`),
        new Parameter(`ScriptBlock`, `Specifies the commands to run. Enclose the commands in braces ( { } ) to create a script block. This parameter is required.`, `ScriptBlock`),
    ], `Registers a custom argument completer.`, `Register-ArgumentCompleter -CommandName <String[]> [-Native] -ScriptBlock <ScriptBlock> [<CommonParameters>]

Register-ArgumentCompleter [-CommandName <String[]>] -ParameterName <String> -ScriptBlock <ScriptBlock> [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Register-CimIndicationEvent`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Register-EngineEvent`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Register-ObjectEvent`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Register-PackageSource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Register-PSSessionConfiguration`, [
        new Parameter(`AccessMode`, `Enables and disables the session configuration and determines whether it can be used for remote or local sessions on the computer. The acceptable values for this parameter are:

- Disabled. Disables the session configuration. It cannot be used for remote or local access to the computer. - Local. Allows users of the local computer to use the session configuration to create a local loopback session on the same computer, but denies access to remote users. - Remote. Allows local and remote users to use the session configuration to create sessions and run commands on this computer.

The default value is Remote.

Other cmdlets can override the value of this parameter later. For example, the Enable-PSRemoting cmdlet allows for remote access to all session configurations, the Enable-PSSessionConfiguration cmdlet enables session configurations, and the Disable-PSRemoting cmdlet prevents remote access to all session configurations.

This parameter was introduced in Windows PowerShell 3.0.`, `PSSessionConfigurationAccessMode`),
        new Parameter(`ApplicationBase`, `Specifies the path of the assembly file ( .dll) that is specified in the value of the AssemblyName* parameter. Use this parameter when the value of the AssemblyName parameter does not include a path. The default is the current directory.`, `String`),
        new Parameter(`AssemblyName`, `Specifies the name of an assembly file (*.dll) in which the configuration type is defined. You can specify the path of the .dll in this parameter or in the value of the ApplicationBase parameter.

This parameter is required when you specify the ConfigurationTypeName parameter.`, `String`),
        new Parameter(`ConfigurationTypeName`, `Specifies the fully qualified name of the Microsoft .NET Framework type that is used for this configuration. The type that you specify must implement the System.Management.Automation.Remoting.PSSessionConfiguration class.

To specify the assembly file (.dll) that implements the configuration type, specify the AssemblyName and ApplicationBase parameters.

Creating a type lets you control more aspects of the session configuration, such as exposing or hiding certain parameters of cmdlets, or setting data size and object size limits that users cannot override.

If you omit this parameter, the DefaultRemotePowerShellConfiguration class is used for the session configuration.`, `String`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Force`, `Suppresses all user prompts and restarts the WinRM service without prompting. Restarting the service makes the configuration change effective.

To prevent a restart and suppress the restart prompt, specify the NoServiceRestart parameter.`, `SwitchParameter`),
        new Parameter(`MaximumReceivedDataSizePerCommandMB`, `Specifies a limit for the amount of data that can be sent to this computer in any single remote command. Enter the data size in megabytes (MB). The default is 50 MB.

If a data size limit is defined in the configuration type that is specified in the ConfigurationTypeName parameter, the limit in the configuration type is used and the value of this parameter is ignored.`, `Double`),
        new Parameter(`MaximumReceivedObjectSizeMB`, `Specifies a limit for the amount of data that can be sent to this computer in any single object. Enter the data size in megabytes. The default is 10 MB.

If an object size limit is defined in the configuration type that is specified in the ConfigurationTypeName parameter, the limit in the configuration type is used and the value of this parameter is ignored.`, `Double`),
        new Parameter(`ModulesToImport`, `Specifies the modules and snap-ins that are automatically imported into sessions that use the session configuration.

By default, only the Microsoft.PowerShell.Core snap-in is imported into sessions. Unless the cmdlets are excluded, you can use the Import-Module and Add-PSSnapin cmdlets to add modules and snap-ins to the session.

The modules specified in this parameter value are imported in additions to modules that are specified by the SessionType parameter and those listed in the ModulesToImport key in the session configuration file (New-PSSessionConfigurationFile). However, settings in the session configuration file can hide the commands exported by modules or prevent users from using them.

This parameter was introduced in Windows PowerShell 3.0.`, `Object[]`),
        new Parameter(`Name`, `Specifies a name for the session configuration. This parameter is required.`, `String`),
        new Parameter(`NoServiceRestart`, `Does not restart the WinRM service, and suppresses the prompt to restart the service.

By default, when you run a Register-PSSessionConfiguration command, you are prompted to restart the WinRM service to make the new session configuration effective. Until the WinRM service is restarted, the new session configuration is not effective.

To restart the WinRM service without prompting, specify the Force parameter. To restart the WinRM service manually, use the Restart-Service cmdlet.`, `SwitchParameter`),
        new Parameter(`PSVersion`, `Specifies the version of PowerShell in sessions that use this session configuration.

The value of this parameter takes precedence over the value of the PowerShellVersion key in the session configuration file.

This parameter was introduced in Windows PowerShell 3.0.`, `Version`),
        new Parameter(`Path`, `Specifies the path and file name of a session configuration file (.pssc), such as one created by the New-PSSessionConfigurationFile cmdlet. If you omit the path, the default is the current directory.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`ProcessorArchitecture`, `Determines whether a 32-bit or 64-bit version of the PowerShell process is started in sessions that use this session configuration. The acceptable values for this parameter are: x86 (32-bit) and AMD64 (64-bit). The default value is determined by the processor architecture of the computer that hosts the session configuration.

You can use this parameter to create a 32-bit session on a 64-bit computer. Attempts to create a 64-bit process on a 32-bit computer fail.`, `String`),
        new Parameter(`RunAsCredential`, `Specifies credentials for commands in the session. By default, commands run with the permissions of the current user.

This parameter was introduced in Windows PowerShell 3.0.`, `PSCredential`),
        new Parameter(`SecurityDescriptorSddl`, `Specifies a Security Descriptor Definition Language (SDDL) string for the configuration.

This string determines the permissions that are required to use the new session configuration. To use a session configuration in a session, users must have at least Execute(Invoke) permission for the configuration.

If the security descriptor is complex, consider using the ShowSecurityDescriptorUI parameter instead of this parameter. You cannot use both parameters in the same command.

If you omit this parameter, the root SDDL for the WinRM service is used for this configuration. To view or change the root SDDL, use the WSMan provider. For example "Get-Item wsman:\localhost\service\rootSDDL". For more information about the WSMan provider, type "Get-Help wsman".`, `String`),
        new Parameter(`SessionType`, `Specifies the type of session that is created by using the session configuration. The acceptable values for this parameter are:

- Empty. No modules or snap-ins are added to session by default. Use the parameters of this cmdlet to add modules, functions, scripts, and other features to the session. - Default. Adds the Microsoft.PowerShell.Core snap-in to the session. This module includes the Import-Module and Add-PSSnapin cmdlets that users can use to import other modules and snap-ins unless you explicitly prohibit the use of the cmdlets. - RestrictedRemoteServer. Includes only the following cmdlets: Exit-PSSession, Get-Command, Get-FormatData, Get-Help, Measure-Object, Out-Default, and Select-Object. Use a script or assembly, or the keys in the session configuration file, to add modules, functions, scripts, and other features to the session.

The default value is Default.

The value of this parameter takes precedence over the value of the SessionType key in the session configuration file.

This parameter was introduced in Windows PowerShell 3.0.`, `PSSessionType`),
        new Parameter(`SessionTypeOption`, `Specifies type-specific options for the session configuration. Enter a session type options object, such as the PSWorkflowExecutionOption object that the New-PSWorkflowExecutionOption cmdlet returns.

The options of sessions that use the session configuration are determined by the values of session options and the session configuration options. Unless specified, options set in the session, such as by using the New-PSSessionOption cmdlet, take precedence over options set in the session configuration. However, session option values cannot exceed maximum values set in the session configuration.

This parameter was introduced in Windows PowerShell 3.0.`, `PSSessionTypeOption`),
        new Parameter(`ShowSecurityDescriptorUI`, `Indicates that this cmdlet displays a property sheet that helps you create the SDDL for the session configuration. The property sheet appears after you enter the Register-PSSessionConfiguration command and then restart the WinRM service.

When setting the permissions for the configuration, remember that users must have at least Execute(Invoke) permission to use the session configuration in a session.

You cannot use the SecurityDescriptorSDDL parameter and this parameter in the same command.`, `SwitchParameter`),
        new Parameter(`StartupScript`, `Specifies the fully qualified path of a PowerShell script. The specified script runs in the new session that uses the session configuration.

You can use the script to additionally configure the session. If the script generates an error, even a non-terminating error, the session is not created and the New-PSSession command fails.`, `String`),
        new Parameter(`ThreadApartmentState`, `Specifies the apartment state of the threads in the session. The acceptable values for this parameter are: STA, MTA, and Unknown. The default value is Unknown.`, `ApartmentState`),
        new Parameter(`ThreadOptions`, `Specifies how threads are created and used when a command runs in the session. The acceptable values for this parameter are:

- Default

- ReuseThread

- UseCurrentThread

- UseNewThread



The default value is UseCurrentThread.

For more information, see "PSThreadOptions Enumeration" in the Microsoft Developer Network (MSDN) library.`, `PSThreadOptions`),
        new Parameter(`TransportOption`, `Specifies the transport option.

This parameter was introduced in Windows PowerShell 3.0.`, `PSTransportOption`),
        new Parameter(`UseSharedProcess`, `Use only one process to host all sessions that are started by the same user and use the same session configuration. By default, each session is hosted in its own process.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Creates and registers a new session configuration.`, `Register-PSSessionConfiguration [-Name] <String> [-AccessMode {Disabled | Local | Remote}] [-ApplicationBase <String>] [-Confirm] [-Force] [-MaximumReceivedDataSizePerCommandMB <Double>] [-MaximumReceivedObjectSizeMB <Double>] [-ModulesToImport <Object[]>] [-NoServiceRestart] [-PSVersion <Version>] [-ProcessorArchitecture {x86 | amd64}] [-RunAsCredential <PSCredential>] [-SecurityDescriptorSddl <String>] [-SessionType {DefaultRemoteShell | Workflow}] [-SessionTypeOption <PSSessionTypeOption>] [-ShowSecurityDescriptorUI] [-StartupScript <String>] [-ThreadApartmentState {STA | MTA | Unknown}] [-ThreadOptions {Default | UseNewThread | ReuseThread | UseCurrentThread}] [-TransportOption <PSTransportOption>] [-UseSharedProcess] [-WhatIf] [<CommonParameters>]

Register-PSSessionConfiguration [-Name] <String> [-AssemblyName] <String> [-ConfigurationTypeName] <String> [-AccessMode {Disabled | Local | Remote}] [-ApplicationBase <String>] [-Confirm] [-Force] [-MaximumReceivedDataSizePerCommandMB <Double>] [-MaximumReceivedObjectSizeMB <Double>] [-ModulesToImport <Object[]>] [-NoServiceRestart] [-PSVersion <Version>] [-ProcessorArchitecture {x86 | amd64}] [-RunAsCredential <PSCredential>] [-SecurityDescriptorSddl <String>] [-SessionTypeOption <PSSessionTypeOption>] [-ShowSecurityDescriptorUI] [-StartupScript <String>] [-ThreadApartmentState {STA | MTA | Unknown}] [-ThreadOptions {Default | UseNewThread | ReuseThread | UseCurrentThread}] [-TransportOption <PSTransportOption>] [-UseSharedProcess] [-WhatIf] [<CommonParameters>]

Register-PSSessionConfiguration [-Name] <String> [-AccessMode {Disabled | Local | Remote}] [-Confirm] [-Force] [-MaximumReceivedDataSizePerCommandMB <Double>] [-MaximumReceivedObjectSizeMB <Double>] [-NoServiceRestart] -Path <String> [-ProcessorArchitecture {x86 | amd64}] [-RunAsCredential <PSCredential>] [-SecurityDescriptorSddl <String>] [-ShowSecurityDescriptorUI] [-StartupScript <String>] [-ThreadApartmentState {STA | MTA | Unknown}] [-ThreadOptions {Default | UseNewThread | ReuseThread | UseCurrentThread}] [-TransportOption <PSTransportOption>] [-UseSharedProcess] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-Alias`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-CimInstance`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-CimSession`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-Event`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-Item`, [
        new Parameter(`Stream`, `Specifies an alternative data stream from a file that this cmdlet deletes. This cmdlet does not delete the file. Enter the stream name. Wildcard characters are supported.

This parameter is not valid on folders.

The Stream parameter is a dynamic parameter that the FileSystem provider adds to Remove-Item . This parameter works only in file system drives.

You can use Remove-Item to delete an alternative data stream. However, it is not the recommended way to eliminate security checks that block files that are downloaded from the Internet. If you verify that a downloaded file is safe, use the Unblock-File cmdlet.

This parameter was introduced in Windows PowerShell 3.0.`, `String[]`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies items that this cmdlet omits. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the format or language of the provider. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcard characters, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when it retrieves the objects, instead of having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Forces the cmdlet to remove items that cannot otherwise be changed, such as hidden or read-only files or read-only aliases or variables. The cmdlet cannot remove constant aliases or variables. Implementation varies from provider to provider. For more information, see about_Providers. Even using the Force parameter, the cmdlet cannot override security restrictions.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies items to delete. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies a path of the items being removed. Unlike Path , the value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Path`, `Specifies a path of the items being removed. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Recurse`, `Indicates that this cmdlet deletes the items in the specified locations and in all child items of the locations.

When it is used with the Include parameter, the Recurse parameter might not delete all subfolders or all child items. This is a known issue. As a workaround, try piping results of the "Get-ChildItem -Recurse" command to Remove-Item , as described in Example 4 in this topic.`, `SwitchParameter`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Deletes the specified items.`, `Remove-Item [-Stream <String[]>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-Recurse] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Remove-Item [-Path] <String[]> [-Stream <String[]>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-Recurse] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-ItemProperty`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies items that this cmdlet omits. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the format or language of the provider. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcard characters, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when it retrieves the objects instead of having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Forces the cmdlet to remove a property of an object that cannot otherwise be accessed by the user. Implementation varies from provider to provider. For more information, see about_Providers.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies items to delete. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies a path of the item property. The value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Name`, `Specifies the names of the properties to remove.`, `String[]`),
        new Parameter(`Path`, `Specifies the path of the item whose properties are being removed. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Deletes the property and its value from an item.`, `Remove-ItemProperty [-Name] <String[]> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Remove-ItemProperty [-Path] <String[]> [-Name] <String[]> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-Job`, [
        new Parameter(`Command`, `Specifies an array of words that appear in commands. This cmdlet deletes jobs that include the specified words.`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Filter`, `Specifies a hash table of conditions. This cmdlet deletes jobs that satisfy all of the conditions. Enter a hash table where the keys are job properties and the values are job property values.

This parameter works only on custom job types, such as workflow jobs and scheduled jobs. It does not work on standard background jobs, such as those created by using the Start-Job cmdlet. For information about support for this parameter, see the help topic for the job type.

This parameter was introduced in Windows PowerShell 3.0.`, `Hashtable`),
        new Parameter(`Force`, `Indicates that this cmdlet deletes a job even if the status is Running. By default, this cmdlet does not delete running jobs.`, `SwitchParameter`),
        new Parameter(`Id`, `Specifies an array of IDs of background jobs that this cmdlet deletes.

The ID is an integer that uniquely identifies the job in the current session. It is easier to remember and type than the instance ID, but it is unique only in the current session. You can type one or more IDs, separated by commas. To find the ID of a job, type "Get-Job".`, `Int32[]`),
        new Parameter(`InstanceId`, `Specifies an array of instance IDs of jobs that this cmdlet deletes.

An instance ID is a GUID that uniquely identifies the job on the computer. To find the instance ID of a job, use the Get-Job cmdlet or display the job object.`, `Guid[]`),
        new Parameter(`Job`, `Specifies the jobs to be deleted. Enter a variable that contains the jobs or a command that gets the jobs. You can also use a pipeline operator to submit jobs to this cmdlet.`, `Job[]`),
        new Parameter(`Name`, `Specifies an array of friendly names of jobs that this cmdlet deletes. Wildcard characters are permitted.

Because the friendly name is not guaranteed to be unique, even in the session, use the WhatIf and Confirm parameters when you delete jobs by name.`, `String[]`),
        new Parameter(`State`, `Specifies the state of jobs to delete. The acceptable values for this parameter are:

- NotStarted

- Running

- Completed

- Failed

- Stopped

- Blocked

- Disconnected

- Suspending

- Stopping

- Suspended



To delete jobs with a state of Running, use the Force parameter.`, `JobState`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Deletes a PowerShell background job.`, `Remove-Job [-Command <String[]>] [-Confirm] [-WhatIf] [<CommonParameters>]

Remove-Job [-Filter] <Hashtable> [-Confirm] [-Force] [-WhatIf] [<CommonParameters>]

Remove-Job [-Id] <Int32[]> [-Confirm] [-Force] [-WhatIf] [<CommonParameters>]

Remove-Job [-Job] <Job[]> [-Confirm] [-Force] [-WhatIf] [<CommonParameters>]

Remove-Job [-InstanceId] <Guid[]> [-Confirm] [-Force] [-WhatIf] [<CommonParameters>]

Remove-Job [-Name] <String[]> [-Confirm] [-Force] [-WhatIf] [<CommonParameters>]

Remove-Job [-State] {NotStarted | Running | Completed | Failed | Stopped | Blocked | Suspended | Disconnected | Suspending | Stopping | AtBreakpoint} [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-Module`, [
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Force`, `Indicates that this cmdlet removes read-only modules. By default, Remove-Module removes only read-write modules.

The ReadOnly and ReadWrite values are stored in AccessMode property of a module.`, `SwitchParameter`),
        new Parameter(`FullyQualifiedName`, `Specifies the fully qualified names of modules to remove.`, `ModuleSpecification[]`),
        new Parameter(`ModuleInfo`, `Specifies the module objects to remove. Enter a variable that contains a module object ( PSModuleInfo ) or a command that gets a module object, such as a Get-Module command. You can also pipe module objects to Remove-Module .`, `PSModuleInfo[]`),
        new Parameter(`Name`, `Specifies the names of modules to remove. Wildcard characters are permitted. You can also pipe name strings to Remove-Module .`, `String[]`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Removes modules from the current session.`, `Remove-Module [-FullyQualifiedName] <ModuleSpecification[]> [-Confirm] [-Force] [-WhatIf] [<CommonParameters>]

Remove-Module [-ModuleInfo] <PSModuleInfo[]> [-Confirm] [-Force] [-WhatIf] [<CommonParameters>]

Remove-Module [-Name] <String[]> [-Confirm] [-Force] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-PSBreakpoint`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-PSDrive`, [
        new Parameter(`Force`, `Removes the current PowerShell drive.`, `SwitchParameter`),
        new Parameter(`LiteralName`, `Specifies the name of the drive.

The value of LiteralName is used exactly as typed. No characters are interpreted as wildcards. If the name includes escape characters, enclose it in single quotation marks ('). Single quotation marks instruct PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Name`, `Specifies the names of the drives to remove. Do not type a colon (:) after the drive name.`, `String[]`),
        new Parameter(`PSProvider`, `Specifies an array of PSProvider objects. This cmdlet removes and disconnects all of the drives associated with the specified PowerShell provider.`, `String[]`),
        new Parameter(`Scope`, `Specifies a scope for the drive. The acceptable values for this parameter are: Global, Local, and Script, or a number relative to the current scope. Scopes number 0 through the number of scopes. The current scope number is 0 and its parent is 1. For more information, see about_Scopes (../Microsoft.PowerShell.Core/About/about_Scopes.md).`, `String`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Deletes temporary PowerShell drives and disconnects mapped network drives.`, `Remove-PSDrive [-LiteralName] <String[]> [-Force] [-PSProvider <String[]>] [-Scope <String>] [-Confirm] [-WhatIf] [<CommonParameters>]

Remove-PSDrive [-Name] <String[]> [-Force] [-PSProvider <String[]>] [-Scope <String>] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-PSReadlineKeyHandler`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-PSSession`, [
        new Parameter(`ComputerName`, `Specifies an array of names of computers. This cmdlet closes the PSSessions that are connected to the specified computers. Wildcard characters are permitted.

Type the NetBIOS name, an IP address, or a fully qualified domain name of one or more remote computers. To specify the local computer, type the computer name, localhost, or a dot (.).`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Id`, `Specifies an array of IDs of sessions. This cmdlet closes the PSSessions with the specified IDs. Type one or more IDs, separated by commas, or use the range operator (..) to specify a range of IDs.

An ID is an integer that uniquely identifies the PSSession in the current session. It is easier to remember and type than the InstanceId , but it is unique only in the current session. To find the ID of a PSSession , run the Get-PSSession cmdlet without parameters.`, `Int32[]`),
        new Parameter(`InstanceId`, `Specifies an array of instance IDs. This cmdlet closes the PSSessions that have the specified instance IDs.

The instance ID is a GUID that uniquely identifies a PSSession in the current session. The instance ID is unique, even when you have multiple sessions running on a single computer.

The instance ID is stored in the InstanceID property of the object that represents a PSSession . To find the InstanceID of the PSSessions in the current session, type "Get-PSSession | Format-Table Name, ComputerName, InstanceId".`, `Guid[]`),
        new Parameter(`Name`, `Specifies an array of friendly names of sessions. This cmdlet closes the PSSessions that have the specified friendly names. Wildcard characters are permitted.

Because the friendly name of a PSSession might not be unique, when you use the Name parameter, consider also using the WhatIf or Confirm parameter in the Remove-PSSession command.`, `String[]`),
        new Parameter(`Session`, `Specifies the session objects of the PSSessions to close. Enter a variable that contains the PSSessions or a command that creates or gets the PSSessions , such as a New-PSSession or Get-PSSession command. You can also pipe one or more session objects to Remove-PSSession .`, `PSSession[]`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`ContainerId`, `Specifies an array of IDs of containers. This cmdlet starts an interactive session with each of the specified containers. To see the containers that are available to you, use the Get-Container cmdlet.`, `String[]`),
        new Parameter(`VMId`, `Specifies an array of ID of virtual machines. This cmdlet starts an interactive session with each of the specified virtual machines. To see the virtual machines that are available to you, use the following command:

"Get-VM | Select-Object -Property Name, ID"`, `Guid[]`),
        new Parameter(`VMName`, `Specifies an array of names of virtual machines. This cmdlet starts an interactive session with each of the specified virtual machines. To see the virtual machines that are available to you, use the Get-VM cmdlet.`, `String[]`),
    ], `Closes one or more PowerShell sessions (PSSessions).`, `Remove-PSSession [-ComputerName] <String[]> [-Confirm] [-WhatIf] [<CommonParameters>]

Remove-PSSession [-Id] <Int32[]> [-Confirm] [-WhatIf] [<CommonParameters>]

Remove-PSSession [-Confirm] -InstanceId <Guid[]> [-WhatIf] [<CommonParameters>]

Remove-PSSession [-Confirm] -Name <String[]> [-WhatIf] [<CommonParameters>]

Remove-PSSession [-Session] <PSSession[]> [-Confirm] [-WhatIf] [<CommonParameters>]

Remove-PSSession [-Confirm] [-WhatIf] -ContainerId <String[]> [<CommonParameters>]

Remove-PSSession [-Confirm] [-WhatIf] -VMId <Guid[]> [<CommonParameters>]

Remove-PSSession [-Confirm] [-WhatIf] -VMName <String[]> [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-Service`, [
        new Parameter(`InputObject`, `Specifies ServiceController objects that represent the services to stop. Enter a variable that contains the objects, or type a command or expression that gets the objects.`, `ServiceController[]`),
        new Parameter(`Name`, `Specifies the service names of the services to stop. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Removes a Windows service.`, `Remove-Service [-InputObject] <ServiceController[]> [-Confirm] [-WhatIf] [<CommonParameters>]

Remove-Service [-Name] <String[]> [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-TypeData`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-Variable`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Remove-WSManInstance`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Rename-Computer`, [
        new Parameter(`ComputerName`, `Renames the specified remote computer. The default is the local computer.

Type the NetBIOS name, an IP address, or a fully qualified domain name of a remote computer. To specify the local computer, type the computer name, a dot (.), or localhost.

This parameter does not rely on PowerShell remoting. You can use the ComputerName parameter of Rename-Computer even if your computer is not configured to run remote commands.`, `String`),
        new Parameter(`DomainCredential`, `Specifies a user account that has permission to connect to the domain. Explicit credentials are required to rename a computer that is joined to a domain.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.

To specify a user account that has permission to connect to the computer that is specified by the ComputerName parameter, use the LocalCredential parameter.`, `PSCredential`),
        new Parameter(`Force`, `Forces the command to run without asking for user confirmation.`, `SwitchParameter`),
        new Parameter(`LocalCredential`, `Specifies a user account that has permission to connect to the computer specified by the ComputerName parameter. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.

To specify a user account that has permission to connect to the domain, use the DomainCredential parameter.`, `PSCredential`),
        new Parameter(`NewName`, `Specifies a new name for the computer. This parameter is required. The name cannot include control characters, leading or trailing spaces, or any of the following characters: / \\ [ ].`, `String`),
        new Parameter(`PassThru`, `Returns the results of the command. Otherwise, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Restart`, `Indicates that this cmdlet restarts the computer that was renamed. A restart is often required to make the change effective.`, `SwitchParameter`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`Protocol`, `Specifies which protocol to use to rename the computer. The acceptable values for this parameter are: WSMan and DCOM. The default value is DCOM.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`WsmanAuthentication`, `Specifies the mechanism that is used to authenticate the user credentials when this cmdlet uses the WSMan protocol. The acceptable values for this parameter are:

- Basic

- CredSSP

- Default

- Digest

- Kerberos

- Negotiate



The default value is Default.

For more information about the values of this parameter, see AuthenticationMechanism Enumeration (https://msdn.microsoft.com/library/system.management.automation.runspaces.authenticationmechanism)in the MSDN library.

Caution: Credential Security Service Provider (CredSSP) authentication, in which the user credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
    ], `Renames a computer.`, `Rename-Computer [-NewName] <String> [-ComputerName <String>] [-DomainCredential <PSCredential>] [-Force] [-LocalCredential <PSCredential>] [-PassThru] [-Restart] [-Confirm] [-WhatIf] [-Protocol <String>] [-WsmanAuthentication <String>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Rename-Item`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Force`, `Forces the cmdlet to rename items that cannot otherwise be changed, such as hidden or read-only files or read-only aliases or variables. The cmdlet cannot change constant aliases or variables. Implementation varies from provider to provider. For more information, see about_Providers.

Even using the Force parameter, the cmdlet cannot override security restrictions.`, `SwitchParameter`),
        new Parameter(`LiteralPath`, `Specifies the path of the item to rename.

Unlike the Path parameter, the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String`),
        new Parameter(`NewName`, `Specifies the new name of the item. Enter only a name, not a path and name. If you enter a path that differs from the path that is specified in the Path parameter, Rename-Item generates an error. To rename and move an item, use Move-Item .

You cannot use wildcard characters in the value of the NewName parameter. To specify a name for multiple files, use the Replace operator in a regular expression. For more information about the Replace operator, see about_Comparison_Operators.`, `String`),
        new Parameter(`PassThru`, `Returns an object that represents the item to the pipeline. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies the path of the item to rename.`, `String`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Renames an item in a PowerShell provider namespace.`, `Rename-Item [-NewName] <String> [-Credential <PSCredential>] [-Force] -LiteralPath <String> [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Rename-Item [-Path] <String> [-NewName] <String> [-Credential <PSCredential>] [-Force] [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Rename-ItemProperty`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies items that this cmdlet omits. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the format or language of the provider. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcard characters, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when it retrieves the objects instead of having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Forces the cmdlet to rename a property of an object that cannot otherwise be accessed by the user. Implementation varies from provider to provider. For more information, see about_Providers.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies only those items upon which the cmdlet acts, excluding all others.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies a path of the item property. This cmdlet uses the value of the LiteralPath cmdlet exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String`),
        new Parameter(`Name`, `Specifies the current name of the property to rename.`, `String`),
        new Parameter(`NewName`, `Specifies the new name for the property.`, `String`),
        new Parameter(`PassThru`, `Returns an object that represents the item property. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies the path of the item to rename.`, `String`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Renames a property of an item.`, `Rename-ItemProperty [-Name] <String> [-NewName] <String> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String> [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Rename-ItemProperty [-Path] <String> [-Name] <String> [-NewName] <String> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Resolve-Path`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or pass a PSCredential object. You can create a PSCredential object using the "Get-Credential" cmdlet. If you type a user name, this cmdlet prompts you for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`LiteralPath`, `Specifies the path to be resolved. The value of the LiteralPath parameter is used exactly as typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Path`, `Specifies the PowerShell path to resolve. This parameter is required. You can also pipe a path string to "Resolve-Path".`, `String[]`),
        new Parameter(`Relative`, `Indicates that this cmdlet returns a relative path.`, `SwitchParameter`),
    ], `Resolves the wildcard characters in a path, and displays the path contents.`, `Resolve-Path [-Credential <PSCredential>] -LiteralPath <String[]> [-Relative] [<CommonParameters>]

Resolve-Path [-Path] <String[]> [-Credential <PSCredential>] [-Relative] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Restart-Computer`, [
        new Parameter(`AsJob`, `Indicates that this cmdlet runs as a background job.

To use this parameter, the local and remote computers must be configured for remoting and, on Windows Vista and later versions of the Windows operating system, you must open PowerShell by using the Run as administrator option. For more information, see about_Remote_Requirements (../Microsoft.PowerShell.Core/About/about_Remote_Requirements.md).

When you specify the AsJob parameter, the command immediately returns an object that represents the background job. You can continue to work in the session while the job finishes. The job is created on the local computer and the results from remote computers are automatically returned to the local computer. To manage the job, use the Job cmdlets. To get the job results, use the Receive-Job cmdlet.

For more information about PowerShell background jobs, see about_Jobs (../Microsoft.PowerShell.Core/About/about_Jobs.md) and [about_Remote_Jobs](../Microsoft.PowerShell.Core/About/about_Remote_Jobs.md).`, `SwitchParameter`),
        new Parameter(`ComputerName`, `Specifies one or more computers. The default is the local computer.

Type the NETBIOS name, an IP address, or a fully qualified domain name of a remote computer. To specify the local computer, type the computer name, a dot (.), or localhost.

This parameter does not rely on PowerShell remoting. You can use the ComputerName parameter even if your computer is not configured to run remote commands.`, `String[]`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet.`, `PSCredential`),
        new Parameter(`Delay`, `Determines how often, in seconds, PowerShell queries the service that is specified by the For parameter to determine whether it is available after the computer is restarted. Specify a delay between queries, in seconds. The default value is 5 seconds.

This parameter is valid only together with the Wait and For parameters.

This parameter was introduced in Windows PowerShell 3.0.`, `Int16`),
        new Parameter(`For`, `Specifies the behavior of PowerShell as it waits for the specified service or feature to become available after the computer restarts. This parameter is valid only with the Wait parameter.

The acceptable values for this parameter are:

- Default. Waits for PowerShell to restart. - PowerShell. Can run commands in a PowerShell remote session on the computer. - WMI. Receives a reply to a Win32_ComputerSystem query for the computer. - WinRM. Can establish a remote session to the computer by using WS-Management.

This parameter was introduced in Windows PowerShell 3.0.`, `WaitForServiceTypes`),
        new Parameter(`Force`, `Forces an immediate restart of the computers.`, `SwitchParameter`),
        new Parameter(`Impersonation`, `Specifies the impersonation level that this cmdlet uses to call WMI. Restart-Computer uses WMI. The acceptable values for this parameter are:

-- Default. Default impersonation. - Anonymous. Hides the identity of the caller. - Identify. Allows objects to query the credentials of the caller. - Impersonate. Allows objects to use the credentials of the caller.

The default value is Impersonate.`, `ImpersonationLevel`),
        new Parameter(`ThrottleLimit`, `Specifies the maximum number of concurrent connections that can be established to run this command. If you omit this parameter or enter a value of 0, the default value, 32, is used.

The throttle limit applies only to the current command, not to the session or to the computer.`, `Int32`),
        new Parameter(`Timeout`, `Specifies the duration of the wait, in seconds. When the time-out elapses, Restart-Computer returns the command prompt, even if the computers are not restarted. The default value, -1, represents an indefinite time-out.

The Timeout parameter is valid only with the Wait parameter.

This parameter was introduced in Windows PowerShell 3.0.`, `Int32`),
        new Parameter(`Wait`, `Indicates that this cmdlet suppresses the PowerShell prompt and blocks the pipeline until all of the computers have restarted. You can use this parameter in a script to restart computers and then continue to process when the restart is finished.

By default, Wait waits indefinitely for the computers to restart, but you can use Timeout to adjust the timing and the For and Delay parameters to wait for particular services to be available on the restarted computers.

The Wait parameter is not valid when you are restarting the local computer. If the value of the ComputerName parameter contains the names of remote computers and the local computer, Restart-Computer generates a non-terminating error for Wait on the local computer, but it waits for the remote computers to restart.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`DcomAuthentication`, `Specifies the authentication level that is used for the WMI connection. The acceptable values for this parameter are:

- Call. Call-level COM authentication - Connect. Connect-level COM authentication - Default. Windows Authentication - None. No COM authentication - Packet. Packet-level COM authentication - PacketIntegrity. Packet Integrity-level COM authentication - PacketPrivacy. Packet Privacy-level COM authentication - Unchanged. The authentication level is the same as the previous command

The default value is Packet.

For more information about the values of this parameter, see AuthenticationLevel Enumeration (https://msdn.microsoft.com/library/system.management.authenticationlevel)in the MSDN library.

This parameter was introduced in Windows PowerShell 3.0.`, `AuthenticationLevel`),
        new Parameter(`Protocol`, `Specifies which protocol to use to restart the computers. The acceptable values for this parameter are: WSMan and DCOM. The default value is DCOM.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`WsmanAuthentication`, `Specifies the mechanism that is used to authenticate the user credentials when you use the WSMan protocol.

The acceptable values for this parameter are: Basic, CredSSP, Default, Digest, Kerberos, and Negotiate. The default value is Default. For more information about the values of this parameter, see AuthenticationMechanism Enumeration (https://msdn.microsoft.com/library/system.management.automation.runspaces.authenticationmechanism)in the MSDN library.

Caution: Credential Security Service Provider (CredSSP) authentication, in which the user credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Restarts ("reboots") the operating system on local and remote computers.`, `Restart-Computer [[-ComputerName] <String[]>] [[-Credential] <PSCredential>] [-AsJob] [-Force] [-Impersonation {Default | Anonymous | Identify | Impersonate | Delegate}] [-ThrottleLimit <Int32>] [-DcomAuthentication {Default | None | Connect | Call | Packet | PacketIntegrity | PacketPrivacy | Unchanged}] [-Confirm] [-WhatIf] [<CommonParameters>]

Restart-Computer [[-ComputerName] <String[]>] [[-Credential] <PSCredential>] [-Delay <Int16>] [-For {PowerShell | WinRM | Wmi}] [-Force] [-Impersonation {Default | Anonymous | Identify | Impersonate | Delegate}] [-Timeout <Int32>] [-Wait] [-DcomAuthentication {Default | None | Connect | Call | Packet | PacketIntegrity | PacketPrivacy | Unchanged}] [-Protocol {DCOM | WSMan}] [-WsmanAuthentication {Default | Basic | Negotiate | CredSSP | Digest | Kerberos}] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Restart-Service`, [
        new Parameter(`DisplayName`, `Specifies the display names of services to restarted. Wildcard carachters are permitted.`, `String[]`),
        new Parameter(`Exclude`, `Specifies services that this cmdlet omits. The value of this parameter qualifies the Name parameter. Enter a name element or pattern, such as s*. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Force`, `Restarts a service that has dependent services.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies services that this cmdlet restarts. The value of this parameter qualifies the Name parameter. Enter a name element or pattern, such as s*. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`InputObject`, `Specifies ServiceController objects that represent the services to restart. Enter a variable that contains the objects, or type a command or expression that gets the objects.`, `ServiceController[]`),
        new Parameter(`Name`, `Specifies the service names of the services to restart.`, `String[]`),
        new Parameter(`PassThru`, `Returns an object that represents the service. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Stops and then starts one or more services.`, `Restart-Service -DisplayName <String[]> [-Exclude <String[]>] [-Force] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]

Restart-Service [-InputObject] <ServiceController[]> [-Exclude <String[]>] [-Force] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]

Restart-Service [-Name] <String[]> [-Exclude <String[]>] [-Force] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Resume-Service`, [
        new Parameter(`DisplayName`, `Specifies the display names of the services to be resumed. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Exclude`, `Specifies services that this cmdlet omits. The value of this parameter qualifies the Name parameter. Enter a name element or pattern, such as s*. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Include`, `Specifies services to resume. The value of this parameter qualifies Name parameter. Enter a name element or pattern, such as s*. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`InputObject`, `Specifies ServiceController objects that represent the services to resumed. Enter a variable that contains the objects, or type a command or expression that gets the objects.`, `ServiceController[]`),
        new Parameter(`Name`, `Specifies the service names of the services to be resumed.`, `String[]`),
        new Parameter(`PassThru`, `Returns an object that represents the service. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Resumes one or more suspended (paused) services.`, `Resume-Service -DisplayName <String[]> [-Exclude <String[]>] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]

Resume-Service [-InputObject] <ServiceController[]> [-Exclude <String[]>] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]

Resume-Service [-Name] <String[]> [-Exclude <String[]>] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Save-Help`, [
        new Parameter(`Credential`, `Specifies a user credential. This cmdlet runs the command by using credentials of a user who has permission to access the file system location specified by the DestinationPath parameter. This parameter is valid only when the DestinationPath or LiteralPath parameter is used in the command.

This parameter enables you to run Save-Help commands that use the DestinationPath parameter on remote computers. By providing explicit credentials, you can run the command on a remote computer and access a file share on a third computer without encountering an access denied error or using CredSSP authentication to delegate credentials.`, `PSCredential`),
        new Parameter(`DestinationPath`, `Specifies the path of the folder in which the help files are saved. Do not specify a file name or file name extension.`, `String[]`),
        new Parameter(`Force`, `Indicates that this cmdlet does not follow the once-per-day limitation, skips version checking, and downloads files that exceed the 1 GB limit.

Without this parameter, only one Save-Help command for each module is permitted in each 24-hour period, downloads are limited to 1 GB of uncompressed content per module, and help files for a module are installed only when they are newer than the files on the computer.

The once-per-day limit protects the servers that host the help files, and makes it practical for you to add a Save-Help command to your PowerShell profile.

To save help for a module in multiple UI cultures without the Force parameter, include all UI cultures in the same command, such as: "Save-Help -Module PSScheduledJobs -UICulture en-US, fr-FR, pt-BR"`, `SwitchParameter`),
        new Parameter(`FullyQualifiedModule`, `Specifies modules with names that are specified in the form of ModuleSpecification objects. This is described in the Remarks section of ModuleSpecification Constructor (Hashtable) (https://msdn.microsoft.com/library/jj136290)in the MSDN library. For example, the FullyQualifiedModule parameter accepts a module name that is specified in the format @{ModuleName = "modulename"; ModuleVersion = "version_number"} or @{ModuleName = "modulename"; ModuleVersion = "version_number"; Guid = "GUID"}. ModuleName and ModuleVersion are required, but Guid is optional.

You cannot specify the FullyQualifiedModule parameter in the same command as a Module parameter.`, `ModuleSpecification[]`),
        new Parameter(`LiteralPath`, `Specifies a path of the destination folder. Unlike the value of the DestinationPath parameter, the value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Module`, `Specifies modules for which this cmdlet downloads help. Enter one or more module names or name patters in a comma-separated list or in a file that has one module name on each line. Wildcard characters are permitted. You can also pipe module objects from the Get-Module cmdlet to Save-Help .

By default, Save-Help downloads help for all modules that support Updatable Help and are installed on the local computer in a location listed in the PSModulePath environment variable.

To save help for modules that are not installed on the computer, run a Get-Module command on a remote computer. Then pipe the resulting module objects to the Save-Help cmdlet or submit the module objects as the value of the Module or InputObject parameters.

If the module that you specify is installed on the computer, you can enter the module name or a module object. If the module is not installed on the computer, you must enter a module object, such as one returned by the Get-Module cmdlet.

The Module parameter of the Save-Help cmdlet does not accept the full path of a module file or module manifest file. To save help for a module that is not in a PSModulePath location, import the module into the current session before you run the Save-Help command.

A value of "*" (all) attempts to update help for all modules that are installed on the computer. This includes modules that do not support Updatable Help. This value might generate errors when the command encounters modules that do not support Updatable Help.`, `PSModuleInfo[]`),
        new Parameter(`UICulture`, `Specifies UI culture values for which this cmdlet gets updated help files. Enter one or more language codes, such as es-ES, a variable that contains culture objects, or a command that gets culture objects, such as a Get-Culture or Get-UICulture command.

Wildcard characters are not permitted. Do not specify a partial language code, such as "de".

By default, Save-Help gets help files in the UI culture set for Windows or its fallback culture. If you specify the UICulture parameter, Save-Help looks for help only for the specified UI culture, not in any fallback culture.`, `CultureInfo[]`),
        new Parameter(`UseDefaultCredentials`, `Indicates that this cmdlet runs the command, including the web download, with the credentials of the current user. By default, the command runs without explicit credentials.

This parameter is effective only when the web download uses NTLM, negotiate, or Kerberos-based authentication.`, `SwitchParameter`),
    ], `Downloads and saves the newest help files to a file system directory.`, `Save-Help [-DestinationPath] <String[]> [[-Module] <PSModuleInfo[]>] [[-UICulture] <CultureInfo[]>] [-Credential <PSCredential>] [-Force] [-FullyQualifiedModule <ModuleSpecification[]>] [-UseDefaultCredentials] [<CommonParameters>]

Save-Help [[-Module] <PSModuleInfo[]>] [[-UICulture] <CultureInfo[]>] [-Credential <PSCredential>] [-Force] [-FullyQualifiedModule <ModuleSpecification[]>] -LiteralPath <String[]> [-UseDefaultCredentials] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Save-Package`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Select-Object`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Select-String`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Select-Xml`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Send-MailMessage`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-Acl`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-Alias`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-AuthenticodeSignature`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-CimInstance`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-Content`, [
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as "User01" or "Domain01\User01", or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, you will be prompted for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Encoding`, `Specifies the file encoding. The acceptable values for this parameter are:

- ASCII Uses the encoding for the ASCII (7-bit) character set. - BigEndianUnicode Encodes in UTF-16 format using the big-endian byte order. - BigEndianUTF32 Encodes in UTF-32 format using the big-endian byte order. - Default Encodes using the default value: ASCII. - Byte Encodes a set of characters into a sequence of bytes. - String Uses the encoding type for a string. - Unicode Encodes in UTF-16 format using the little-endian byte order. - UTF7 Encodes in UTF-7 format. - UTF8 Encodes in UTF-8 format. - Unknown The encoding type is unknown or invalid; the data can be treated as binary.

The default value is ASCII.

Encoding is a dynamic parameter that the FileSystem provider adds to Set-Content . This parameter works only in file system drives.`, `FileSystemCmdletProviderEncoding`),
        new Parameter(`Exclude`, `Omits the specified items. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as "*.txt". Wildcards are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the format or language of the provider. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcard characters, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when it is retrieving the objects, instead of having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Forces the cmdlet to set the contents of a file, even if the file is read-only. Implementation varies from provider to provider. For more information, see about_Providers. Using the Force parameter does not override security restrictions.`, `SwitchParameter`),
        new Parameter(`Include`, `Changes only the specified items. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as "*.txt". Wildcards are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies the path of the item that receives the content. Unlike Path, the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`NoNewline`, `The string representations of the input objects are concatenated to form the output. No spaces or newlines are inserted between the output strings. No newline is added after the last output string.`, `SwitchParameter`),
        new Parameter(`PassThru`, `Returns an object that represents the content. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies the path of the item that receives the content. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Stream`, `Specifies an alternative data stream for content. If the stream does not exist, this cmdlet creates it. Wildcard characters are not supported.

Stream is a dynamic parameter that the FileSystem provider adds to "Set-Content". This parameter works only in file system drives.

You can use the "Set-Content" cmdlet to change the content of the Zone.Identifier alternate data stream. However, we do not recommend this as a way to eliminate security checks that block files that are downloaded from the Internet. If you verify that a downloaded file is safe, use the Unblock-File cmdlet.

This parameter was introduced in PowerShell 3.0.`, `String`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see about_Transactions.`, `SwitchParameter`),
        new Parameter(`Value`, `Specifies the new content for the item.`, `Object[]`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Writes or replaces the content in an item with new content.`, `Set-Content [-Value] <Object[]> [-Confirm] [-Credential <PSCredential>] [-Encoding {Unknown | String | Unicode | Byte | BigEndianUnicode | UTF8 | UTF7 | UTF32 | Ascii | Default | Oem | BigEndianUTF32}] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-NoNewline] [-PassThru] [-Stream <String>] [-UseTransaction] [-WhatIf] [<CommonParameters>]

Set-Content [-Path] <String[]> [-Value] <Object[]> [-Confirm] [-Credential <PSCredential>] [-Encoding {Unknown | String | Unicode | Byte | BigEndianUnicode | UTF8 | UTF7 | UTF32 | Ascii | Default | Oem | BigEndianUTF32}] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-NoNewline] [-PassThru] [-Stream <String>] [-UseTransaction] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-Date`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-ExecutionPolicy`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-Item`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts for a password.

This parameter is not supported by any providers installed with parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies items that this cmdlet omits. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the format or language of the provider. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcard characters, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when it retrieves the objects, instead of having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Forces the cmdlet to set items that cannot otherwise be changed, such as read-only alias or variables. The cmdlet cannot change constant aliases or variables. Implementation varies from provider to provider. For more information, see about_Providers. Even using the Force parameter, the cmdlet cannot override security restrictions.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies items that this cmdlet changes. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as *.txt. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`LiteralPath`, `Specifies a path of the location of the new items. Unlike Path , the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`PassThru`, `Passes an object that represents the item to the pipeline. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies a path of the location of the new items. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Value`, `Specifies a new value for the item.`, `Object`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Changes the value of an item to the value specified in the command.`, `Set-Item [[-Value] <Object>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Set-Item [-Path] <String[]> [[-Value] <Object>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-ItemProperty`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.

This parameter is not supported by any providers installed with parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies those items upon which the cmdlet does not act, and includes all others.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the format or language of the provider. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcard characters, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when it retrieves the objects instead of having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Force`, `Forces the cmdlet to set a property on items that cannot otherwise be accessed by the user. Implementation varies from provider to provider. For more information, see about_Providers.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies only those items upon which the cmdlet acts, which excludes all others.`, `String[]`),
        new Parameter(`InputObject`, `Specifies the object that has the properties that this cmdlet changes. Enter a variable that contains the object or a command that gets the object.`, `PSObject`),
        new Parameter(`LiteralPath`, `Specifies a path of the item property. The value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Name`, `Specifies the name of the property.`, `String`),
        new Parameter(`PassThru`, `Returns an object that represents the item property. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies the path of the items with the property to modify.`, `String[]`),
        new Parameter(`Value`, `Specifies the value of the property.`, `Object`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Creates or changes the value of a property of an item.`, `Set-ItemProperty [-Path] <String[]> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -InputObject <PSObject> [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Set-ItemProperty [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -InputObject <PSObject> -LiteralPath <String[]> [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Set-ItemProperty [-Name] <String> [-Value] <Object> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] -LiteralPath <String[]> [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]

Set-ItemProperty [-Path] <String[]> [-Name] <String> [-Value] <Object> [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Force] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-Location`, [
        new Parameter(`LiteralPath`, `Specifies a path of the location. The value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String`),
        new Parameter(`PassThru`, `Returns a System.Management.Automation.PathInfo object that represents the location. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Path`, `Specify the path of a new working location. If no path is provided, "Set-Location" will default to the current user's home directory.`, `String`),
        new Parameter(`StackName`, `Specifies the location stack name that this cmdlet makes the current location stack. Enter a location stack name. To indicate the unnamed default location stack, type "$null" or an empty string ("").

The " -Location" cmdlets act on the current stack unless you use the StackName * parameter to specify a different stack.`, `String`),
    ], `Sets the current working location to a specified location.`, `Set-Location -LiteralPath <String> [-PassThru] [<CommonParameters>]

Set-Location [[-Path] <String>] [-PassThru] [<CommonParameters>]

Set-Location [-PassThru] [-StackName <String>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-PackageSource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-PSBreakpoint`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-PSDebug`, [
        new Parameter(`Off`, `Indicates that this cmdlet turns off all script debugging features.

A "Set-StrictMode -Off" command disables the verification set by a "Set-PSDebug -Strict" command. For more information, see Set-StrictMode.`, `SwitchParameter`),
        new Parameter(`Step`, `Indicates that this cmdlet turns on script stepping. Before each line runs, PowerShell prompts you to stop, continue, or enter a new interpreter level to inspect the state of the script.

Specifying the Step parameter automatically sets a trace level of 1.`, `SwitchParameter`),
        new Parameter(`Strict`, `Indicates that PowerShell returns an exception if a variable is referenced before a value is assigned to the variable.

A "Set-StrictMode -Off" command disables the verification set by a "Set-PSDebug -Strict" command. For more information, see Set-StrictMode .`, `SwitchParameter`),
        new Parameter(`Trace`, `Specifies the trace level. The acceptable values for this parameter are:

- 1: Trace script lines as they run.

- 0: Turn script tracing off.

- 2: Trace script lines, variable assignments, function calls, and scripts.`, `Int32`),
    ], `Turns script debugging features on and off, sets the trace level, and toggles strict mode.`, `Set-PSDebug [-Off] [<CommonParameters>]

Set-PSDebug [-Step] [-Strict] [-Trace <Int32>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-PSReadlineKeyHandler`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-PSReadlineOption`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-PSSessionConfiguration`, [
        new Parameter(`AccessMode`, `Enables and disables the session configuration and determines whether it can be used for remote or local sessions on the computer. The acceptable values for this parameter are:

- Disabled. Disables the session configuration. It cannot be used for remote or local access to the computer. This value sets the Enabled property of the session configuration (WSMan:\<ComputerName>\PlugIn\<SessionConfigurationName>\Enabled) to False. - Local. Adds a Network_Deny_All entry to security descriptor of the session configuration. Users of the local computer can use the session configuration to create a local loopback session on the same computer, but remote users are denied access. - Remote. Removes Deny_All and Network_Deny_All entries from the security descriptors of the session configuration. Users of local and remote computers can use the session configuration to create sessions and run commands on this computer.

The default value is Remote.

Other cmdlets can override the value of this parameter later. For example, the Enable-PSRemoting cmdlet enables all session configurations on the computer and permits remote access to them, and the Disable-PSRemoting cmdlet permits only local access to all session configurations on the computer.

This parameter was introduced in Windows PowerShell 3.0.`, `PSSessionConfigurationAccessMode`),
        new Parameter(`ApplicationBase`, `Specifies the path of the assembly file ( .dll) that is specified in the value of the AssemblyName* parameter.`, `String`),
        new Parameter(`AssemblyName`, `Specifies the assembly name. This cmdlet creates a session configuration based on a class that is defined in an assembly.

Enter the file name or full path of an assembly .dll file that defines a session configuration. If you enter only the file name, you can enter the path in the value of the ApplicationBase parameter.`, `String`),
        new Parameter(`ConfigurationTypeName`, `Specifies the type of the session configuration that is defined in the assembly in the AssemblyName parameter. The type that you specify must implement the System.Management.Automation.Remoting.PSSessionConfiguration class.

This parameter is required when you specify an assembly name.`, `String`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Force`, `Suppresses all user prompts, and restarts the WinRM service without prompting. Restarting the service makes the configuration change effective.

To prevent a restart and suppress the restart prompt, use the NoServiceRestart parameter.`, `SwitchParameter`),
        new Parameter(`MaximumReceivedDataSizePerCommandMB`, `Specifies the limit on the amount of data that can be sent to this computer in any single remote command. Enter the data size in megabytes (MB). The default is 50 MB.

If a data size limit is defined in the configuration type that is specified in the ConfigurationTypeName parameter, the limit in the configuration type is used. The value of this parameter is ignored.`, `Double`),
        new Parameter(`MaximumReceivedObjectSizeMB`, `Specifies the limits on the amount of data that can be sent to this computer in any single object. Enter the data size in megabytes. The default is 10 MB.

If an object size limit is defined in the configuration type that is specified in the ConfigurationTypeName parameter, the limit in the configuration type is used. The value of this parameter is ignored.`, `Double`),
        new Parameter(`ModulesToImport`, `Specifies the modules and snap-ins that are automatically imported into sessions that use the session configuration. Enter the module and snap-in names.

By default, only the Microsoft.PowerShell.Core snap-in is imported into sessions, but unless the cmdlets are excluded, you can use the Import-Module and Add-PSSnapin cmdlets to add modules and snap-ins to the session.

The modules specified in this parameter value are imported in additions to modules specified in the session configuration file ( New-PSSessionConfigurationFile ). However, settings in the session configuration file can hide the commands exported by modules or prevent users from using them.

The modules specified in this parameter value replace the list of modules specified by using the ModulesToImport parameter of the Register-PSSessionConfiguration cmdlet.

This parameter was introduced in Windows PowerShell 3.0.`, `Object[]`),
        new Parameter(`Name`, `Specifies the name of the session configuration that you want to change.

You cannot use this parameter to change the name of the session configuration.`, `String`),
        new Parameter(`NoServiceRestart`, `Does not restart the WinRM service, and suppresses the prompt to restart the service.

By default, when you run Set-PSSessionConfiguration , you are prompted to restart the WinRM service to make the new session configuration effective. Until the WinRM service is restarted, the new session configuration is not effective.

To restart the WinRM service without prompting, use the Force parameter. To restart the WinRM service manually, use the Restart-Service cmdlet.`, `SwitchParameter`),
        new Parameter(`PSVersion`, `Specifies the version of PowerShell in sessions that use this session configuration.

The value of this parameter takes precedence over the value of the PowerShellVersion key in the session configuration file.

This parameter was introduced in Windows PowerShell 3.0.`, `Version`),
        new Parameter(`Path`, `Specifies the path of a session configuration file (.pssc), such as one created by the New-PSSessionConfigurationFile cmdlet. If you omit the path, the default is the current directory.

For information about how to modify a session configuration file, see the help topic for the New-PSSessionConfigurationFile cmdlet.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`RunAsCredential`, `Specifies credentials for commands in the session. By default, commands run with the permissions of the current user.

This parameter was introduced in Windows PowerShell 3.0.`, `PSCredential`),
        new Parameter(`SecurityDescriptorSddl`, `Specifies a different Security Descriptor Definition Language (SDDL) string for the configuration.

This string determines the permissions that are required to use the new session configuration. To use a session configuration in a session, users must have at least Execute(Invoke) permission for the configuration.

To use the default security descriptor for the configuration, enter an empty string ("") or a value of $Null. The default is the root SDDL in the WSMan: drive.

If the security descriptor is complex, consider using the ShowSecurityDescriptorUI parameter instead of this one. You cannot use both parameters in the same command.`, `String`),
        new Parameter(`SessionTypeOption`, `Specifies type-specific options for the session configuration. Enter a session type options object, such as the PSWorkflowExecutionOption object that the New-PSWorkflowExecutionOption cmdlet returns.

The options of sessions that use the session configuration are determined by the values of session options and the session configuration options. Unless specified, options set in the session, such as by using the New-PSSessionOption cmdlet, take precedence over options set in the session configuration. However, session option values cannot exceed maximum values set in the session configuration.

This parameter was introduced in Windows PowerShell 3.0.`, `PSSessionTypeOption`),
        new Parameter(`ShowSecurityDescriptorUI`, `Indicates that this cmdlet a property sheet that helps you create a new SDDL for the session configuration. The property sheet appears after you run the Set-PSSessionConfiguration command and then restart the WinRM service.

When you set permissions to the configuration, remember that users must have at least Execute(Invoke) permission to use the session configuration in a session.

You cannot use the SecurityDescriptorSDDL parameter and this parameter in the same command.`, `SwitchParameter`),
        new Parameter(`StartupScript`, `Specifies the startup script for the configuration. Enter the fully qualified path of a PowerShell script. The specified script runs in the new session that uses the session configuration.

To delete a startup script from a session configuration, enter an empty string ("") or a value of $Null.

You can use a startup script to further configure the user session. If the script generates an error, even a non-terminating error, the session is not created and the New-PSSession command fails.`, `String`),
        new Parameter(`ThreadApartmentState`, `Specifies the apartment state setting for the threads in the session. The acceptable values for this parameter are: STA, MTA, and Unknown. The default value is Unknown.`, `ApartmentState`),
        new Parameter(`ThreadOptions`, `Specifies the thread options setting in the configuration. This setting defines how threads are created and used when a command is executed in the session. The acceptable values for this parameter are:

- Default

- ReuseThread

- UseCurrentThread

- UseNewThread



The default value is UseCurrentThread.

For more information, see "PSThreadOptions Enumeration" in the Microsoft Developer Network (MSDN) library.`, `PSThreadOptions`),
        new Parameter(`TransportOption`, `Specifies the transport options for the session configuration. Enter a transport options object, such as the WSManConfigurationOption object that the New-PSTransportOption cmdlet returns.

The options of sessions that use the session configuration are determined by the values of session options and the session configuration options. Unless specified, options set in the session, such as by using the New-PSSessionOption cmdlet, take precedence over options set in the session configuration. However, session option values cannot exceed maximum values set in the session configuration.

This parameter was introduced in Windows PowerShell 3.0.`, `PSTransportOption`),
        new Parameter(`UseSharedProcess`, `Use only one process to host all sessions that are started by the same user and use the same session configuration. By default, each session is hosted in its own process.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Changes the properties of a registered session configuration.`, `Set-PSSessionConfiguration [-Name] <String> [-AccessMode {Disabled | Local | Remote}] [-ApplicationBase <String>] [-Confirm] [-Force] [-MaximumReceivedDataSizePerCommandMB <Double>] [-MaximumReceivedObjectSizeMB <Double>] [-ModulesToImport <Object[]>] [-NoServiceRestart] [-PSVersion <Version>] [-RunAsCredential <PSCredential>] [-SecurityDescriptorSddl <String>] [-SessionTypeOption <PSSessionTypeOption>] [-ShowSecurityDescriptorUI] [-StartupScript <String>] [-ThreadApartmentState {STA | MTA | Unknown}] [-ThreadOptions {Default | UseNewThread | ReuseThread | UseCurrentThread}] [-TransportOption <PSTransportOption>] [-UseSharedProcess] [-WhatIf] [<CommonParameters>]

Set-PSSessionConfiguration [-Name] <String> [-AssemblyName] <String> [-ConfigurationTypeName] <String> [-AccessMode {Disabled | Local | Remote}] [-ApplicationBase <String>] [-Confirm] [-Force] [-MaximumReceivedDataSizePerCommandMB <Double>] [-MaximumReceivedObjectSizeMB <Double>] [-ModulesToImport <Object[]>] [-NoServiceRestart] [-PSVersion <Version>] [-RunAsCredential <PSCredential>] [-SecurityDescriptorSddl <String>] [-SessionTypeOption <PSSessionTypeOption>] [-ShowSecurityDescriptorUI] [-StartupScript <String>] [-ThreadApartmentState {STA | MTA | Unknown}] [-ThreadOptions {Default | UseNewThread | ReuseThread | UseCurrentThread}] [-TransportOption <PSTransportOption>] [-UseSharedProcess] [-WhatIf] [<CommonParameters>]

Set-PSSessionConfiguration [-Name] <String> [-AccessMode {Disabled | Local | Remote}] [-Confirm] [-Force] [-MaximumReceivedDataSizePerCommandMB <Double>] [-MaximumReceivedObjectSizeMB <Double>] [-NoServiceRestart] -Path <String> [-RunAsCredential <PSCredential>] [-SecurityDescriptorSddl <String>] [-ShowSecurityDescriptorUI] [-StartupScript <String>] [-ThreadApartmentState {STA | MTA | Unknown}] [-ThreadOptions {Default | UseNewThread | ReuseThread | UseCurrentThread}] [-TransportOption <PSTransportOption>] [-UseSharedProcess] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-Service`, [
        new Parameter(`Description`, `Specifies a new description for the service.

The service description appears in Services in Computer Management. Description is not a property of the ServiceController object that "Get-Service" gets. To see the service description, use "Get-CimInstance" to get a Win32_Service object that represents the service.`, `String`),
        new Parameter(`DisplayName`, `Specifies a new display name for the service.`, `String`),
        new Parameter(`InputObject`, `Specifies a ServiceController object that represents the service to change. Enter a variable that contains the object, or type a command or expression that gets the object, such as a "Get-Service" command. You can also pipe a service object to "Set-Service".`, `ServiceController`),
        new Parameter(`Name`, `Specifies the service name of the service to be changed. Wildcard characters are not permitted. You can also pipe a service name to "Set-Service".`, `String`),
        new Parameter(`Credential`, `Specifies the credentials under which the service should be run.`, `PSCredential`),
        new Parameter(`PassThru`, `Returns objects that represent the services that were changed. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`StartupType`, `Specifies the start mode of the service. The acceptable values for this parameter are:

- Automatic.   Start when the system starts. - Manual.   Starts only when started by a user or program. - Disabled.   Cannot be started.`, `ServiceStartMode`),
        new Parameter(`Status`, `Specifies the status for the service. The acceptable values for this parameter are:

- Running.   Starts the service. - Stopped.   Stops the service. - Paused.   Suspends the service.`, `String`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Starts, stops, and suspends a service, and changes its properties.`, `Set-Service [-Description <String>] [-DisplayName <String>] [-InputObject <ServiceController>] [-Credential <PSCredential>] [-PassThru] [-StartupType {Boot | System | Automatic | Manual | Disabled}] [-Status {Running | Stopped | Paused}] [-Confirm] [-WhatIf] [<CommonParameters>]

Set-Service [-Name] <String> [-Description <String>] [-DisplayName <String>] [-Credential <PSCredential>] [-PassThru] [-StartupType {Boot | System | Automatic | Manual | Disabled}] [-Status {Running | Stopped | Paused}] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-StrictMode`, [
        new Parameter(`Off`, `Indicates that this cmdlet turns strict mode off. This parameter also turns off "Set-PSDebug -Strict".`, `SwitchParameter`),
        new Parameter(`Version`, `Specifies the conditions that cause an error in strict mode.

The acceptable values for this parameter are:

- 1.0

---- Prohibits references to uninitialized variables, except for uninitialized variables in strings.

- 2.0



---- Prohibits references to uninitialized variables. This includes uninitialized variables in strings.

---- Prohibits references to non-existent properties of an object.

---- Prohibits function calls that use the syntax for calling methods.

---- Prohibits a variable without a name ().



Latest

---- Selects the latest version available. The latest version is the most strict. Use this value to make sure that scripts use the strictest available version, even when new versions are added to PowerShell.`, `Version`),
    ], `Establishes and enforces coding rules in expressions, scripts, and script blocks.`, `Set-StrictMode -Off [<CommonParameters>]

Set-StrictMode -Version <Version> [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-TimeZone`, [
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Id`, `Specifies the ID of the time zone that this cmdlet sets.`, `String`),
        new Parameter(`InputObject`, `Specifies a TimeZoneInfo object to use as input.`, `TimeZoneInfo`),
        new Parameter(`Name`, `Specifies the name of the time zone that this cmdlet sets.`, `String`),
        new Parameter(`PassThru`, `Returns an object representing the item with which you are working. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Sets the system time zone to a specified time zone.`, `Set-TimeZone [-Confirm] -Id <String> [-PassThru] [-WhatIf] [<CommonParameters>]

Set-TimeZone [-InputObject] <TimeZoneInfo> [-Confirm] [-PassThru] [-WhatIf] [<CommonParameters>]

Set-TimeZone [-Name] <String> [-Confirm] [-PassThru] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-TraceSource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-Variable`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-WSManInstance`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Set-WSManQuickConfig`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Sort-Object`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Split-Path`, [
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.

This parameter is not supported by any providers installed with parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Extension`, `Indicates that this cmdlet returns only the extension of the leaf. For example, in the path "C:\Test\Logs\Pass1.log", it returns only ".log".`, `SwitchParameter`),
        new Parameter(`IsAbsolute`, `Indicates that this cmdlet returns $True if the path is absolute and $False if it is relative. An absolute path has a length greater than zero and does not use a dot (.) to indicate the current path.`, `SwitchParameter`),
        new Parameter(`Leaf`, `Indicates that this cmdlet returns only the last item or container in the path. For example, in the path "C:\Test\Logs\Pass1.log", it returns only Pass1.log.`, `SwitchParameter`),
        new Parameter(`LeafBase`, `Indicates that this cmdlet returns only base name of the leaf. For example, in the path "C:\Test\Logs\Pass1.log", it returns only "Pass1".`, `SwitchParameter`),
        new Parameter(`LiteralPath`, `Specifies the paths to be split. Unlike Path , the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`NoQualifier`, `Indicates that this cmdlet returns the path without the qualifier. For the FileSystem or registry providers, the qualifier is the drive of the provider path, such as C: or HKCU:. For example, in the path "C:\Test\Logs\Pass1.log", it returns only \Test\Logs\Pass1.log.`, `SwitchParameter`),
        new Parameter(`Parent`, `Indicates that this cmdlet returns only the parent containers of the item or of the container specified by the path. For example, in the path "C:\Test\Logs\Pass1.log", it returns C:\Test\Logs. The Parent parameter is the default split location parameter.`, `SwitchParameter`),
        new Parameter(`Path`, `Specifies the paths to be split. Wildcard characters are permitted. If the path includes spaces, enclose it in quotation marks. You can also pipe a path to this cmdlet.`, `String[]`),
        new Parameter(`Qualifier`, `Indicates that this cmdlet returns only the qualifier of the specified path. For the FileSystem or registry providers, the qualifier is the drive of the provider path, such as C: or HKCU:.`, `SwitchParameter`),
        new Parameter(`Resolve`, `Indicates that this cmdlet displays the items that are referenced by the resulting split path instead of displaying the path elements.`, `SwitchParameter`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Returns the specified part of a path.`, `Split-Path [-Credential <PSCredential>] [-Extension] [-Resolve] [-UseTransaction] [<CommonParameters>]

Split-Path [-Path] <String[]> [-Credential <PSCredential>] [-IsAbsolute] [-Resolve] [-UseTransaction] [<CommonParameters>]

Split-Path [-Path] <String[]> [-Credential <PSCredential>] [-Leaf] [-Resolve] [-UseTransaction] [<CommonParameters>]

Split-Path [-Credential <PSCredential>] [-LeafBase] [-Resolve] [-UseTransaction] [<CommonParameters>]

Split-Path [-Credential <PSCredential>] -LiteralPath <String[]> [-Resolve] [-UseTransaction] [<CommonParameters>]

Split-Path [-Path] <String[]> [-Credential <PSCredential>] [-NoQualifier] [-Resolve] [-UseTransaction] [<CommonParameters>]

Split-Path [-Path] <String[]> [-Credential <PSCredential>] [-Parent] [-Resolve] [-UseTransaction] [<CommonParameters>]

Split-Path [-Path] <String[]> [[-Qualifier]] [-Credential <PSCredential>] [-Resolve] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Start-Job`, [
        new Parameter(`ArgumentList`, `Specifies an array of arguments, or parameter values, for the script that is specified by the FilePath parameter.

Because all of the values that follow the ArgumentList parameter name are interpreted as being values of ArgumentList , specify this parameter as the last parameter in the command.`, `Object[]`),
        new Parameter(`Authentication`, `Specifies the mechanism that is used to authenticate user credentials. The acceptable values for this parameter are:

- Default

- Basic

- Credssp

- Digest

- Kerberos

- Negotiate

- NegotiateWithImplicitCredential



The default value is Default.

CredSSP authentication is available only in Windows Vista, Windows Server 2008, and later versions of the Windows operating system.

For more information about the values of this parameter, see AuthenticationMechanism Enumeration (https://msdn.microsoft.com/library/system.management.automation.runspaces.authenticationmechanism)in the MSDN library.

Caution: Credential Security Support Provider (CredSSP) authentication, in which the user's credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.`, `AuthenticationMechanism`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one from the Get-Credential cmdlet.`, `PSCredential`),
        new Parameter(`DefinitionName`, `Specifies the definition name of the job that this cmdlet starts. Use this parameter to start custom job types that have a definition name, such as scheduled jobs.

When you use Start-Job to start an instance of a scheduled job, the job starts immediately, regardless of job triggers or job options. The resulting job instance is a scheduled job, but it is not saved to disk like triggered scheduled jobs. Also, you cannot use the ArgumentList parameter of Start-Job to provide values for parameters of scripts that run in a scheduled job. For more information, see about_Scheduled_Jobs.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`DefinitionPath`, `Specifies path of the definition for the job that this cmdlet starts. Enter the definition path. The concatenation of the values of the DefinitionPath and DefinitionName parameters is the fully qualified path of the job definition. Use this parameter to start custom job types that have a definition path, such as scheduled jobs.

For scheduled jobs, the value of the DefinitionPath parameter is "$home\AppData\Local\Windows\PowerShell\ScheduledJob".

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`FilePath`, `Specifies a local script that this cmdlet runs as a background job. Enter the path and file name of the script or pipe a script path to Start-Job . The script must be on the local computer or in a folder that the local computer can access.

When you use this parameter, PowerShell converts the contents of the specified script file to a script block and runs the script block as a background job.`, `String`),
        new Parameter(`InitializationScript`, `Specifies commands that run before the job starts. Enclose the commands in braces ( { } ) to create a script block.

Use this parameter to prepare the session in which the job runs. For example, you can use it to add functions, snap-ins, and modules to the session.`, `ScriptBlock`),
        new Parameter(`InputObject`, `Specifies input to the command. Enter a variable that contains the objects, or type a command or expression that generates the objects.

In the value of the ScriptBlock parameter, use the $Input automatic variable to represent the input objects.`, `PSObject`),
        new Parameter(`LiteralPath`, `Specifies a local script that this cmdlet runs as a background job. Enter the path of a script on the local computer.

Unlike the FilePath parameter, this cmdlet uses the value of the LiteralPath parameter exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String`),
        new Parameter(`Name`, `Specifies a friendly name for the new job. You can use the name to identify the job to other job cmdlets, such as the Stop-Job cmdlet.

The default friendly name is Job#, where # is an ordinal number that is incremented for each job.`, `String`),
        new Parameter(`PSVersion`, `Specifies a version. This cmdlet runs the job with the version of PowerShell. The acceptable values for this parameter are: 2.0 and 3.0.

This parameter was introduced in Windows PowerShell 3.0.`, `Version`),
        new Parameter(`RunAs32`, `Indicates that this cmdlet runs the job in a 32-bit process. Use this parameter to force the job to run in a 32-bit process on a 64-bit operating system.

On 64-bit versions of Windows 7 and Windows Server 2008 R2, when the Start-Job command includes the RunAs32 parameter, you cannot use the Credential parameter to specify the credentials of another user.`, `SwitchParameter`),
        new Parameter(`ScriptBlock`, `Specifies the commands to run in the background job. Enclose the commands in braces ( { } ) to create a script block. This parameter is required.`, `ScriptBlock`),
        new Parameter(`Type`, `Specifies the custom type for jobs that this cmdlet starts. Enter a custom job type name, such as PSScheduledJob for scheduled jobs or PSWorkflowJob for workflows jobs. This parameter is not valid for standard background jobs.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
    ], `Starts a PowerShell background job.`, `Start-Job [-ScriptBlock] <ScriptBlock> [[-InitializationScript] <ScriptBlock>] [-ArgumentList <Object[]>] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-Credential <PSCredential>] [-InputObject <PSObject>] [-Name <String>] [-PSVersion <Version>] [-RunAs32] [<CommonParameters>]

Start-Job [[-InitializationScript] <ScriptBlock>] [-ArgumentList <Object[]>] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-Credential <PSCredential>] [-InputObject <PSObject>] -LiteralPath <String> [-Name <String>] [-PSVersion <Version>] [-RunAs32] [<CommonParameters>]

Start-Job [-FilePath] <String> [[-InitializationScript] <ScriptBlock>] [-ArgumentList <Object[]>] [-Authentication {Default | Basic | Negotiate | NegotiateWithImplicitCredential | Credssp | Digest | Kerberos}] [-Credential <PSCredential>] [-InputObject <PSObject>] [-Name <String>] [-PSVersion <Version>] [-RunAs32] [<CommonParameters>]

Start-Job [-DefinitionName] <String> [[-DefinitionPath] <String>] [[-Type] <String>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Start-Process`, [
        new Parameter(`ArgumentList`, `Specifies parameters or parameter values to use when this cmdlet starts the process. If parameters or parameter values contain a space, they need surrounded with escaped double quotes.`, `String[]`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one from the Get-Credential cmdlet. By default, the cmdlet uses the credentials of the current user.`, `PSCredential`),
        new Parameter(`FilePath`, `Specifies the optional path and file name of the program that runs in the process. Enter the name of an executable file or of a document, such as a .txt or .doc file, that is associated with a program on the computer. This parameter is required.

If you specify only a file name, use the WorkingDirectory parameter to specify the path.`, `String`),
        new Parameter(`LoadUserProfile`, `Indicates that this cmdlet loads the Windows user profile stored in the HKEY_USERS registry key for the current user.

This parameter does not affect the PowerShell profiles. For more information, see about_Profiles.`, `SwitchParameter`),
        new Parameter(`NoNewWindow`, `Start the new process in the current console window. By default PowerShell opens a new window.

You cannot use the NoNewWindow and WindowStyle parameters in the same command.`, `SwitchParameter`),
        new Parameter(`PassThru`, `Returns a process object for each process that the cmdlet started. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`RedirectStandardError`, `Specifies a file. This cmdlet sends any errors generated by the process to a file that you specify. Enter the path and file name. By default, the errors are displayed in the console.`, `String`),
        new Parameter(`RedirectStandardInput`, `Specifies a file. This cmdlet reads input from the specified file. Enter the path and file name of the input file. By default, the process gets its input from the keyboard.`, `String`),
        new Parameter(`RedirectStandardOutput`, `Specifies a file. This cmdlet sends the output generated by the process to a file that you specify. Enter the path and file name. By default, the output is displayed in the console.`, `String`),
        new Parameter(`UseNewEnvironment`, `Indicates that this cmdlet uses new environment variables specified for the process. By default, the started process runs with the environment variables specified for the computer and user.`, `SwitchParameter`),
        new Parameter(`Verb`, `Specifies a verb to use when this cmdlet starts the process. The verbs that are available are determined by the file name extension of the file that runs in the process.

The following table shows the verbs for some common process file types.

| File type | Verbs   | | --------- | ------- | |.cmd       | Edit, Open, Print, Runas | |.exe       | Open, RunAs | |.txt       | Open, Print, PrintTo | |.wav       | Open, Play |

To find the verbs that can be used with the file that runs in a process, use the New-Object cmdlet to create a System.Diagnostics.ProcessStartInfo object for the file. The available verbs are in the Verbs property of the ProcessStartInfo object. For details, see the examples.`, `String`),
        new Parameter(`Wait`, `Indicates that this cmdlet waits for the specified process to complete before accepting more input. This parameter suppresses the command prompt or retains the window until the process finishes.`, `SwitchParameter`),
        new Parameter(`WindowStyle`, `Specifies the state of the window that is used for the new process. The acceptable values for this parameter are: Normal, Hidden, Minimized, and Maximized. The default value is Normal.

You cannot use the WindowStyle and NoNewWindow parameters in the same command.`, `ProcessWindowStyle`),
        new Parameter(`WorkingDirectory`, `Specifies the location of the executable file or document that runs in the process. The default is the folder for the new process.`, `String`),
    ], `Starts one or more processes on the local computer.`, `Start-Process [-FilePath] <String> [[-ArgumentList] <String[]>] [-Credential <PSCredential>] [-LoadUserProfile] [-NoNewWindow] [-PassThru] [-RedirectStandardError <String>] [-RedirectStandardInput <String>] [-RedirectStandardOutput <String>] [-UseNewEnvironment] [-Wait] [-WindowStyle {Normal | Hidden | Minimized | Maximized}] [-WorkingDirectory <String>] [<CommonParameters>]

Start-Process [-FilePath] <String> [[-ArgumentList] <String[]>] [-PassThru] [-Verb <String>] [-Wait] [-WindowStyle {Normal | Hidden | Minimized | Maximized}] [-WorkingDirectory <String>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Start-Service`, [
        new Parameter(`DisplayName`, `Specifies the display names of the services to start. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Exclude`, `Specifies services that this cmdlet omits. The value of this parameter qualifies the Name parameter. Enter a name element or pattern, such as "s*". Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Include`, `Specifies services that this cmdlet starts. The value of this parameter qualifies the Name parameter. Enter a name element or pattern, such as "s*". Wildcard characters are permitted.`, `String[]`),
        new Parameter(`InputObject`, `Specifies ServiceController objects representing the services to be started. Enter a variable that contains the objects, or type a command or expression that gets the objects.`, `ServiceController[]`),
        new Parameter(`Name`, `Specifies the service names for the service to be started.

The parameter name is optional. You can use Name or its alias, ServiceName , or you can omit the parameter name.`, `String[]`),
        new Parameter(`PassThru`, `Returns an object that represents the service. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Starts one or more stopped services.`, `Start-Service -DisplayName <String[]> [-Exclude <String[]>] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]

Start-Service [-InputObject] <ServiceController[]> [-Exclude <String[]>] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]

Start-Service [-Name] <String[]> [-Exclude <String[]>] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Start-Sleep`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Start-Transcript`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Stop-Computer`, [
        new Parameter(`AsJob`, `Indicates that this cmdlet runs as a background job.

To use this parameter, the local and remote computers must be configured for remoting and, on Windows Vista and later versions of the Windows operating system, you must open PowerShell by using the Run as administrator option. For more information, see about_Remote_Requirements.

When you specify the AsJob parameter, the command immediately returns an object that represents the background job. You can continue to work in the session while the job finishes. The job is created on the local computer and the results from remote computers are automatically returned to the local computer. To get the job results, use the Receive-Job cmdlet.

For more information about PowerShell background jobs, see about_Jobs (../Microsoft.PowerShell.Core/About/about_Jobs.md) and [about_Remote_Jobs](../Microsoft.PowerShell.Core/About/about_Remote_Jobs.md).`, `SwitchParameter`),
        new Parameter(`ComputerName`, `Specifies the computers to stop. The default is the local computer.

Type the NETBIOS name, IP address, or fully qualified domain name of one or more computers in a comma-separated list. To specify the local computer, type the computer name or localhost.

This parameter does not rely on PowerShell remoting. You can use the ComputerName parameter even if your computer is not configured to run remote commands.`, `String[]`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01, or enter a PSCredential object, such as one from the Get-Credential cmdlet.`, `PSCredential`),
        new Parameter(`Force`, `Forces an immediate shut down of the computers.`, `SwitchParameter`),
        new Parameter(`Impersonation`, `Specifies the impersonation level to use when this cmdlet calls WMI. Stop-Computer uses WMI. The acceptable values for this parameter are:

- Default. Default impersonation. - Anonymous. Hides the identity of the caller. - Identify. Allows objects to query the credentials of the caller. - Impersonate. Allows objects to use the credentials of the caller.

The default value is Impersonate.`, `ImpersonationLevel`),
        new Parameter(`ThrottleLimit`, `Specifies the maximum number of concurrent connections that can be established to run this command. If you omit this parameter or enter a value of 0, the default value, 32, is used.

The throttle limit applies only to the current command, not to the session or to the computer.`, `Int32`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
        new Parameter(`DcomAuthentication`, `Specifies the authentication level that this cmdlet uses with WMI. Stop-Computer uses WMI. The acceptable values for this parameter are:

- Default. Windows Authentication - None. No COM authentication - Connect. Connect-level COM authentication - Call. Call-level COM authentication - Packet . Packet-level COM authentication - PacketIntegrity. Packet Integrity-level COM authentication - PacketPrivacy. Packet Privacy-level COM authentication - Unchanged. Same as the previous command

The default value is Packet.

For more information about the values of this parameter, see AuthenticationLevel Enumeration (https://msdn.microsoft.com/library/system.management.authenticationlevel)in the MSDN library.`, `AuthenticationLevel`),
        new Parameter(`Protocol`, `Specifies which protocol to use to restart the computers. The acceptable values for this parameter are: WSMan and DCOM. The default value is DCOM.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`WsmanAuthentication`, `Specifies the mechanism that is used to authenticate the user credentials when this cmdlet uses the WSMan protocol. The acceptable values for this parameter are:

- Basic

- CredSSP

- Default

- Digest

- Kerberos

- Negotiate.



The default value is Default.

For more information about the values of this parameter, see AuthenticationMechanism Enumeration (https://msdn.microsoft.com/library/system.management.automation.runspaces.authenticationmechanism)in the MSDN library.

Caution: Credential Security Service Provider (CredSSP) authentication, in which the user credentials are passed to a remote computer to be authenticated, is designed for commands that require authentication on more than one resource, such as accessing a remote network share. This mechanism increases the security risk of the remote operation. If the remote computer is compromised, the credentials that are passed to it can be used to control the network session.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
    ], `Stops (shuts down) local and remote computers.`, `Stop-Computer [[-ComputerName] <String[]>] [[-Credential] <PSCredential>] [-AsJob] [-Force] [-Impersonation {Default | Anonymous | Identify | Impersonate | Delegate}] [-ThrottleLimit <Int32>] [-Confirm] [-WhatIf] [-DcomAuthentication <AuthenticationLevel>] [-Protocol <String>] [-WsmanAuthentication <String>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Stop-Job`, [
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Filter`, `Specifies a hash table of conditions. This cmdlet stops jobs that satisfy all of the conditions. Enter a hash table where the keys are job properties and the values are job property values.

This parameter works only on custom job types, such as workflow jobs and scheduled jobs. It does not work on standard background jobs, such as those created by using the Start-Job cmdlet. For information about support for this parameter, see the help topic for the job type.

This parameter was introduced in Windows PowerShell 3.0.`, `Hashtable`),
        new Parameter(`Id`, `Specifies the IDs of jobs that this cmdlet stops. The default is all jobs in the current session.

The ID is an integer that uniquely identifies the job in the current session. It is easier to remember and type than the instance ID, but it is unique only in the current session. You can type one or more IDs, separated by commas. To find the ID of a job, type "Get-Job".`, `Int32[]`),
        new Parameter(`InstanceId`, `Specifies the instance IDs of jobs that this cmdlet stops. The default is all jobs.

An instance ID is a GUID that uniquely identifies the job on the computer. To find the instance ID of a job, use Get-Job.`, `Guid[]`),
        new Parameter(`Job`, `Specifies the jobs that this cmdlet stops. Enter a variable that contains the jobs or a command that gets the jobs. You can also use a pipeline operator to submit jobs to the Stop-Job cmdlet. By default, Stop-Job deletes all jobs that were started in the current session.`, `Job[]`),
        new Parameter(`Name`, `Specifies friendly names of jobs that this cmdlet stops. Enter the job names in a comma-separated list or use wildcard characters (*) to enter a job name pattern. By default, Stop-Job stops all jobs created in the current session.

Because the friendly name is not guaranteed to be unique, use the WhatIf and Confirm parameters when stopping jobs by name.`, `String[]`),
        new Parameter(`PassThru`, `Returns an object representing the item with which you are working. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`State`, `Specifies a job state. This cmdlet stops only jobs in the specified state. The acceptable values for this parameter are:

- NotStarted

- Running

- Completed

- Failed

- Stopped

- Blocked

- Suspended

- Disconnected

- Suspending

- Stopping



For more information about job states, see JobState Enumeration (https://msdn.microsoft.com/library/system.management.automation.jobstate)in the MSDN library.`, `JobState`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Stops a PowerShell background job.`, `Stop-Job [-Filter] <Hashtable> [-Confirm] [-PassThru] [-WhatIf] [<CommonParameters>]

Stop-Job [-Id] <Int32[]> [-Confirm] [-PassThru] [-WhatIf] [<CommonParameters>]

Stop-Job [-InstanceId] <Guid[]> [-Confirm] [-PassThru] [-WhatIf] [<CommonParameters>]

Stop-Job [-Job] <Job[]> [-Confirm] [-PassThru] [-WhatIf] [<CommonParameters>]

Stop-Job [-Name] <String[]> [-Confirm] [-PassThru] [-WhatIf] [<CommonParameters>]

Stop-Job [-State] {NotStarted | Running | Completed | Failed | Stopped | Blocked | Suspended | Disconnected | Suspending | Stopping | AtBreakpoint} [-Confirm] [-PassThru] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Stop-Process`, [
        new Parameter(`Force`, `Stops the specified processes without prompting for confirmation. By default, Stop-Process prompts for confirmation before stopping any process that is not owned by the current user.

To find the owner of a process, use the Get-WmiObject cmdlet to get a Win32_Process object that represents the process, and then use the GetOwner method of the object.`, `SwitchParameter`),
        new Parameter(`Id`, `Specifies the process IDs of the processes to stop. To specify multiple IDs, use commas to separate the IDs. To find the PID of a process, type "Get-Process".`, `Int32[]`),
        new Parameter(`InputObject`, `Specifies the process objects to stop. Enter a variable that contains the objects, or type a command or expression that gets the objects.`, `Process[]`),
        new Parameter(`Name`, `Specifies the process names of the processes to stop. You can type multiple process names, separated by commas, or use wildcard characters.`, `String[]`),
        new Parameter(`PassThru`, `Returns an object that represents the process. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Stops one or more running processes.`, `Stop-Process [-Id] <Int32[]> [-Force] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]

Stop-Process [-InputObject] <Process[]> [-Force] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]

Stop-Process [-Force] -Name <String[]> [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Stop-Service`, [
        new Parameter(`DisplayName`, `Specifies the display names of the services to stop. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Exclude`, `Specifies services that this cmdlet omits. The value of this parameter qualifies the Name parameter. Enter a name element or pattern, such as s*. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Force`, `Forces the cmdlet to stop a service even if that service has dependent services.`, `SwitchParameter`),
        new Parameter(`Include`, `Specifies services that this cmdlet stops. The value of this parameter qualifies the Name parameter. Enter a name element or pattern, such as s*. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`InputObject`, `Specifies ServiceController objects that represent the services to stop. Enter a variable that contains the objects, or type a command or expression that gets the objects.`, `ServiceController[]`),
        new Parameter(`Name`, `Specifies the service names of the services to stop. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`NoWait`, `Indicates that this cmdlet uses the no wait option.`, `SwitchParameter`),
        new Parameter(`PassThru`, `Returns an object that represents the service. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Stops one or more running services.`, `Stop-Service -DisplayName <String[]> [-Exclude <String[]>] [-Force] [-Include <String[]>] [-NoWait] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]

Stop-Service [-InputObject] <ServiceController[]> [-Exclude <String[]>] [-Force] [-Include <String[]>] [-NoWait] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]

Stop-Service [-Name] <String[]> [-Exclude <String[]>] [-Force] [-Include <String[]>] [-NoWait] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Stop-Transcript`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Suspend-Service`, [
        new Parameter(`DisplayName`, `Specifies the display names of the services to be suspended. Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Exclude`, `Specifies services to omit from the specified services. The value of this parameter qualifies the Name parameter. Enter a name element or pattern, such as "s*". Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Include`, `Specifies services to suspend. The value of this parameter qualifies the Name parameter. Enter a name element or pattern, such as "s*". Wildcard characters are permitted.`, `String[]`),
        new Parameter(`InputObject`, `Specifies ServiceController objects that represent the services to suspend. Enter a variable that contains the objects, or type a command or expression that gets the objects.`, `ServiceController[]`),
        new Parameter(`Name`, `Specifies the service names of the services to suspend. Wildcard characters are permitted.

The parameter name is optional. You can use Name or its alias, ServiceName , or you can omit the parameter name.`, `String[]`),
        new Parameter(`PassThru`, `Returns an object representing the item with which you are working. By default, this cmdlet does not generate any output.`, `SwitchParameter`),
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Suspends (pauses) one or more running services.`, `Suspend-Service -DisplayName <String[]> [-Exclude <String[]>] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]

Suspend-Service [-InputObject] <ServiceController[]> [-Exclude <String[]>] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]

Suspend-Service [-Name] <String[]> [-Exclude <String[]>] [-Include <String[]>] [-PassThru] [-Confirm] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Tee-Object`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Test-FileCatalog`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Test-ModuleManifest`, [
        new Parameter(`Path`, `Specifies a path and file name for the manifest file. Enter an optional path and name of the module manifest file that has the .psd1 file name extension. The default location is the current directory. Wildcard characters are supported, but must resolve to a single module manifest file. This parameter is required. You can also pipe a path to Test-ModuleManifest .`, `String`),
    ], `Verifies that a module manifest file accurately describes the contents of a module.`, `Test-ModuleManifest [-Path] <String> [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Test-Path`, [
        new Parameter(`OlderThan`, `Specify a time as a DateTime object.`, `DateTime`),
        new Parameter(`NewerThan`, `Specify a time as a DateTime object.`, `DateTime`),
        new Parameter(`Credential`, `Specifies a user account that has permission to perform this action. The default is the current user.

Type a user name, such as User01 or Domain01\User01. Or, enter a PSCredential object, such as one generated by the Get-Credential cmdlet. If you type a user name, this cmdlet prompts you for a password.

This parameter is not supported by any providers installed with PowerShell.`, `PSCredential`),
        new Parameter(`Exclude`, `Specifies items that this cmdlet omits. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as "*.txt". Wildcard characters are permitted.`, `String[]`),
        new Parameter(`Filter`, `Specifies a filter in the format or language of the provider. The value of this parameter qualifies the Path parameter. The syntax of the filter, including the use of wildcard characters, depends on the provider. Filters are more efficient than other parameters, because the provider applies them when it retrieves the objects instead of having PowerShell filter the objects after they are retrieved.`, `String`),
        new Parameter(`Include`, `Specifies paths that this cmdlet tests. The value of this parameter qualifies the Path parameter. Enter a path element or pattern, such as "*.txt". Wildcard characters are permitted.`, `String[]`),
        new Parameter(`IsValid`, `Indicates that this cmdlet tests the syntax of the path, regardless of whether the elements of the path exist. This cmdlet returns $True if the path syntax is valid and $False if it is not.`, `SwitchParameter`),
        new Parameter(`LiteralPath`, `Specifies a path to be tested. Unlike Path , the value of the LiteralPath parameter is used exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Path`, `Specifies a path to be tested. Wildcard characters are permitted. If the path includes spaces, enclose it in quotation marks.`, `String[]`),
        new Parameter(`PathType`, `Specifies the type of the final element in the path. This cmdlet returns $True if the element is of the specified type and $False if it is not. The acceptable values for this parameter are:

- Container. An element that contains other elements, such as a directory or registry key. - Leaf. An element that does not contain other elements, such as a file. - Any. Either a container or a leaf. Tells whether the final element in the path is of a particular type.`, `TestPathType`),
        new Parameter(`UseTransaction`, `Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see`, `SwitchParameter`),
    ], `Determines whether all elements of a path exist.`, `Test-Path [-OlderThan <DateTime>] [-NewerThan <DateTime>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Include <String[]>] [-IsValid] -LiteralPath <String[]> [-PathType {Any | Container | Leaf}] [-UseTransaction] [<CommonParameters>]

Test-Path [-Path] <String[]> [-OlderThan <DateTime>] [-NewerThan <DateTime>] [-Credential <PSCredential>] [-Exclude <String[]>] [-Filter <String>] [-Include <String[]>] [-IsValid] [-PathType {Any | Container | Leaf}] [-UseTransaction] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Test-PSSessionConfigurationFile`, [
        new Parameter(`Path`, `Specifies the path and file name of a session configuration file (.pssc). If you omit the path, the default is the current folder. Wildcard characters are supported, but they must resolve to a single file. You can also pipe a session configuration file path to Test-PSSessionConfigurationFile .`, `String`),
    ], `Verifies the keys and values in a session configuration file.`, `Test-PSSessionConfigurationFile [-Path] <String> [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Test-WSMan`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Trace-Command`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Unblock-File`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Uninstall-Package`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Unprotect-CmsMessage`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Unregister-Event`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Unregister-PackageSource`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Unregister-PSSessionConfiguration`, [
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Force`, `Indicates that the cmdlet does not prompt you for confirmation, and restarts the WinRM service without prompting. Restarting the service makes the configuration change effective.

To prevent a restart and suppress the restart prompt, use the NoServiceRestart parameter.`, `SwitchParameter`),
        new Parameter(`Name`, `Specifies the names of the session configurations to delete. Enter one session configuration name or a configuration name pattern. Wildcard characters are permitted. This parameter is required.

You can also pipe a session configurations to Unregister-PSSessionConfiguration .`, `String`),
        new Parameter(`NoServiceRestart`, `Indicates that this cmdlet does not restart the WinRM service, and suppresses the prompt to restart the service.

By default, when you run an Unregister-PSSessionConfiguration command, you are prompted to restart the WinRM service to make the change effective. Until the WinRM service is restarted, users can still use the unregistered session configuration, even though Get-PSSessionConfiguration does not find it.

To restart the WinRM service without prompting, specify the Force parameter. To restart the WinRM service manually, use the Restart-Service cmdlet.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Deletes registered session configurations from the computer.`, `Unregister-PSSessionConfiguration [-Name] <String> [-Confirm] [-Force] [-NoServiceRestart] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Update-FormatData`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Update-Help`, [
        new Parameter(`Confirm`, `Prompts you for confirmation before running the cmdlet.`, `SwitchParameter`),
        new Parameter(`Credential`, `Specifies credentials of a user who has permission to access the file system location specified by SourcePath . This parameter is valid only when the SourcePath or LiteralPath parameter is used in the command.

This parameter enables you to run Update-Help commands that have SourcePath on remote computers. By providing explicit credentials, you can run the command on a remote computer and access a file share on a third computer without encountering an access denied error or using CredSSP authentication to delegate credentials.`, `PSCredential`),
        new Parameter(`Force`, `Indicates that this cmdlet does not follow the once-per-day limitation, skips version checking, and downloads files that exceed the 1 GB limit.

Without this parameter, Update-Help runs only once in each 24-hour period, downloads are limited to 1 GB of uncompressed content per module and help files are installed only when they are newer than the files on the computer.

The once-per-day limit protects the servers that host the help files and makes it practical for you to add an Update-Help command to your PowerShell profile without incurring the resource cost of repeated connections or downloads.

To update help for a module in multiple UI cultures without the Force parameter, include all UI cultures in the same command, such as: "Update-Help -Module PSScheduledJobs -UICulture en-US, fr-FR, pt-BR"`, `SwitchParameter`),
        new Parameter(`FullyQualifiedModule`, `Specifies modules with names that are specified in the form of ModuleSpecification objects. These are described in the Remarks section of ModuleSpecification Constructor (Hashtable) (https://msdn.microsoft.com/library/jj136290)in the MSDN library. For example, the FullyQualifiedModule parameter accepts a module name that is specified in the format @{ModuleName = "modulename"; ModuleVersion = "version_number"} or @{ModuleName = "modulename"; ModuleVersion = "version_number"; Guid = "GUID"}. ModuleName and ModuleVersion are required, but Guid is optional.

You cannot specify the FullyQualifiedModule parameter in the same command as a Module parameter.`, `ModuleSpecification[]`),
        new Parameter(`LiteralPath`, `Specifies the folder for updated help files instead of downloading them from the Internet. Use this parameter or SourcePath if you have used the Save-Help cmdlet to download help files to a directory.

You can also pipe a directory object, such as one from the Get-Item or Get-ChildItem cmdlets, to Update-Help .

Unlike the value of SourcePath , the value of LiteralPath is used exactly as it is typed. No characters are interpreted as wildcard characters. If the path includes escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters as escape sequences.`, `String[]`),
        new Parameter(`Module`, `Specifies modules for which this cmdlet updates help. Enter one or more module names or name patters in a comma-separated list, or specify a file that lists one module name on each line. Wildcard characters are permitted. You can also pipe modules from the Get-Module cmdlet, to the Update-Help cmdlet.

The modules that you specify must be installed on the computer, but they do not have to be imported into the current session. You can specify any module in the session or any module that is installed in a location listed in the PSModulePath environment variable.

A value of * (all) attempts to update help for all modules that are installed on the computer. This includes modules that do not support Updatable Help. This value might generate errors when the command encounters modules that do not support Updatable Help. Instead, run Update-Help without parameters.

The Module parameter of the Update-Help cmdlet does not accept the full path of a module file or module manifest file. To update help for a module that is not in a PSModulePath location, import the module into the current session before you run the Update-Help command.`, `String[]`),
        new Parameter(`Recurse`, `Searches recursively for help files in the specified directory. This parameter is valid only when SourcePath is used in the command.`, `SwitchParameter`),
        new Parameter(`SourcePath`, `Specifies a file system folder from which this cmdlet gets updated help files, instead of downloading them from the Internet. Enter the path of a folder. Do not specify a file name or file name extension. You can also pipe a folder, such as one from the Get-Item or Get-ChildItem cmdlets, to Update-Help .

By default, Update-Help downloads updated help files from the Internet. Use this parameter when you have used the Save-Help cmdlet to download updated help files to a directory.

Administrators can use the Set the default source path for Update-Help Group Policy setting under Computer Configuration to specify a default value for SourcePath . This Group Policy setting prevents users from using Update-Help to download help files from the Internet. For more information, see about_Group_Policy_Settings (http://go.microsoft.com/fwlink/?LinkId=251696).`, `String[]`),
        new Parameter(`UICulture`, `Specifies UI culture values for which this cmdlet gets updated help files. Enter one or more language codes, such as es-ES, a variable that contains culture objects, or a command that gets culture objects, such as a Get-Culture or Get-UICulture command. Wildcard characters are not permitted and you cannot submit a partial language code, such as "de".

By default, Update-Help gets help files in the UI culture set for Windows or its fallback culture. If you specify the UICulture parameter, Update-Help looks for help only for the specified UI culture, not in any fallback culture.

Commands that use the UICulture parameter succeed only when the module provides help files for the specified UI culture. If the command fails because the specified UI culture is not supported, the error message includes a list of UI cultures that the module supports.`, `CultureInfo[]`),
        new Parameter(`UseDefaultCredentials`, `Indicates that this cmdlet runs the command, including the Internet download, by using the credentials of the current user. By default, the command runs without explicit credentials.

This parameter is effective only when the Web download uses NTLM, negotiate, or Kerberos-based authentication.`, `SwitchParameter`),
        new Parameter(`WhatIf`, `Shows what would happen if the cmdlet runs. The cmdlet is not run.`, `SwitchParameter`),
    ], `Downloads and installs the newest help files on your computer.`, `Update-Help [[-Module] <String[]>] [[-UICulture] <CultureInfo[]>] [-Confirm] [-Credential <PSCredential>] [-Force] [-FullyQualifiedModule <ModuleSpecification[]>] [-LiteralPath <String[]>] [-Recurse] [-UseDefaultCredentials] [-WhatIf] [<CommonParameters>]

Update-Help [[-Module] <String[]>] [[-SourcePath] <String[]>] [[-UICulture] <CultureInfo[]>] [-Confirm] [-Credential <PSCredential>] [-Force] [-FullyQualifiedModule <ModuleSpecification[]>] [-Recurse] [-UseDefaultCredentials] [-WhatIf] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Update-TypeData`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Wait-Debugger`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Wait-Event`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Wait-Job`, [
        new Parameter(`Any`, `Indicates that this cmdlet displays the command prompt, and returns the job object, when any job finishes. By default, Wait-Job waits until all of the specified jobs are complete before it displays the prompt.`, `SwitchParameter`),
        new Parameter(`Filter`, `Specifies a hash table of conditions. This cmdlet waits for jobs that satisfy all of the conditions in the hash table. Enter a hash table where the keys are job properties and the values are job property values.

This parameter works only on custom job types, such as workflow jobs and scheduled jobs. It does not work on standard background jobs, such as those created by using the Start-Job cmdlet. For information about support for this parameter, see the help topic for the job type.

This parameter was introduced in Windows PowerShell 3.0.`, `Hashtable`),
        new Parameter(`Force`, `Indicates that this cmdlet continues to wait for jobs in the Suspended or Disconnected state. By default, Wait-Job returns, or ends  the wait, when jobs are in one of the following states:

- Completed

- Failed

- Stopped

- Suspended

- Disconnected



This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`Id`, `Specifies an array of IDs of jobs for which this cmdlet waits.

The ID is an integer that uniquely identifies the job in the current session. It is easier to remember and type than the instance ID, but it is unique only in the current session. You can type one or more IDs, separated by commas. To find the ID of a job, type "Get-Job".`, `Int32[]`),
        new Parameter(`InstanceId`, `Specifies an array of instance IDs of jobs for which this cmdlet waits. The default is all jobs.

An instance ID is a GUID that uniquely identifies the job on the computer. To find the instance ID of a job, use Get-Job .`, `Guid[]`),
        new Parameter(`Job`, `Specifies the jobs for which this cmdlet waits. Enter a variable that contains the job objects or a command that gets the job objects. You can also use a pipeline operator to send job objects to the Wait-Job cmdlet. By default, Wait-Job waits for all jobs created in the current session.`, `Job[]`),
        new Parameter(`Name`, `Specifies friendly names of jobs for which this cmdlet waits.`, `String[]`),
        new Parameter(`State`, `Specifies a job state. This cmdlet waits only for jobs in the specified state. The acceptable values for this parameter are:

- NotStarted

- Running

- Completed

- Failed

- Stopped

- Blocked

- Suspended

- Disconnected

- Suspending

- Stopping



For more information about job states, see JobState Enumeration (https://msdn.microsoft.com/library/system.management.automation.jobstate)in the MSDN library.`, `JobState`),
        new Parameter(`Timeout`, `Specifies the maximum wait time for each background job, in seconds. The default value, -1, indicates that the cmdlet waits until the job finishes. The timing starts when you submit the Wait-Job command, not the Start-Job command.

If this time is exceeded, the wait ends and the command prompt returns, even if the job is still running. The command does not display any error message.`, `Int32`),
    ], `Suppresses the command prompt until one or all of the PowerShell background jobs running in the session are completed.`, `Wait-Job [-Filter] <Hashtable> [-Any] [-Force] [-Timeout <Int32>] [<CommonParameters>]

Wait-Job [-Id] <Int32[]> [-Any] [-Force] [-Timeout <Int32>] [<CommonParameters>]

Wait-Job [-InstanceId] <Guid[]> [-Any] [-Force] [-Timeout <Int32>] [<CommonParameters>]

Wait-Job [-Job] <Job[]> [-Any] [-Force] [-Timeout <Int32>] [<CommonParameters>]

Wait-Job [-Name] <String[]> [-Any] [-Force] [-Timeout <Int32>] [<CommonParameters>]

Wait-Job [-State] {NotStarted | Running | Completed | Failed | Stopped | Blocked | Suspended | Disconnected | Suspending | Stopping | AtBreakpoint} [-Any] [-Force] [-Timeout <Int32>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Wait-Process`, [
        new Parameter(`Id`, `Specifies the process IDs of the processes. To specify multiple IDs, use commas to separate the IDs. To find the PID of a process, type "Get-Process".`, `Int32[]`),
        new Parameter(`InputObject`, `Specifies the processes by submitting process objects. Enter a variable that contains the process objects, or type a command or expression that gets the process objects, such as the Get-Process cmdlet.`, `Process[]`),
        new Parameter(`Name`, `Specifies the process names of the processes. To specify multiple names, use commas to separate the names. Wildcard characters are not supported.`, `String[]`),
        new Parameter(`Timeout`, `Specifies the maximum time, in seconds, that this cmdlet waits for the specified processes to stop. When this interval expires, the command displays a non-terminating error that lists the processes that are still running, and ends the wait. By default, there is no time-out.`, `Int32`),
    ], `Waits for the processes to be stopped before accepting more input.`, `Wait-Process [-Id] <Int32[]> [[-Timeout] <Int32>] [<CommonParameters>]

Wait-Process [[-Timeout] <Int32>] -InputObject <Process[]> [<CommonParameters>]

Wait-Process [-Name] <String[]> [[-Timeout] <Int32>] [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Where-Object`, [
        new Parameter(`CContains`, `Indicates that this cmdlet gets objects from a collection if the property value of the object is an exact match for the specified value. This operation is case-sensitive.

For example: "Get-Process | where ProcessName -CContains "svchost"" CContains refers to a collection of values and is true if the collection contains an item that is an exact match for the specified value. If the input is a single object, PowerShell converts it to a collection of one object.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`CEQ`, `Indicates that this cmdlet gets objects if the property value is the same as the specified value. This operation is case-sensitive.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`CGE`, `Indicates that this cmdlet gets objects if the property value is greater than or equal to the specified value. This operation is case-sensitive.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`CGT`, `Indicates that this cmdlet gets objects if the property value is greater than the specified value. This operation is case-sensitive.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`CIn`, `Indicates that this cmdlet gets objects if the property value includes the specified value. This operation is case-sensitive.

For example: "Get-Process | where -Value "svchost" -CIn ProcessName" CIn resembles CContains , except that the property and value positions are reversed. For example, the following statements are both true.

"abc", "def" -CContains "abc"

"abc" -CIn "abc", "def"

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`CLE`, `Indicates that this cmdlet gets objects if the property value is less-than or equal to the specified value. This operation is case-sensitive.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`CLT`, `Indicates that this cmdlet gets objects if the property value is less-than the specified value. This operation is case-sensitive.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`CLike`, `Indicates that this cmdlet gets objects if the property value matches a value that includes wildcard characters. This operation is case-sensitive.

For example: "Get-Process | where ProcessName -CLike "*host""

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`CMatch`, `Indicates that this cmdlet gets objects if the property value matches the specified regular expression. This operation is case-sensitive. When the input is scalar, the matched value is saved in "$Matches" automatic variable.

For example: "Get-Process | where ProcessName -CMatch "Shell""

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`CNE`, `Indicates that this cmdlet gets objects if the property value is different than the specified value. This operation is case-sensitive.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`CNotContains`, `Indicates that this cmdlet gets objects if the property value of the object is not an exact match for the specified value. This operation is case-sensitive.

For example: "Get-Process | where ProcessName -CNotContains "svchost""

"NotContains" and "CNotContains refer to a collection of values and are true when the collection does not contains any items that are an exact match for the specified value. If the input is a single object, PowerShell converts it to a collection of one object.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`CNotIn`, `Indicates that this cmdlet gets objects if the property value is not an exact match for the specified value. This operation is case-sensitive.

For example: "Get-Process | where -Value "svchost" -CNotIn -Property ProcessName" NotIn and CNotIn operators resemble NotContains and CNotContains , except that the property and value positions are reversed. For example, the following statements are true.

"abc", "def" -CNotContains "Abc"

"abc" -CNotIn "Abc", "def"`, `SwitchParameter`),
        new Parameter(`CNotLike`, `Indicates that this cmdlet gets objects if the property value does not match a value that includes wildcard characters. This operation is case-sensitive.

For example: "Get-Process | where ProcessName -CNotLike "*host""

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`CNotMatch`, `Indicates that this cmdlet gets objects if the property value does not match the specified regular expression. This operation is case-sensitive. When the input is scalar, the matched value is saved in "$Matches" automatic variable.

For example: "Get-Process | where ProcessName -CNotMatch "Shell""

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`Contains`, `Indicates that this cmdlet gets objects if any item in the property value of the object is an exact match for the specified value.

For example: "Get-Process | where ProcessName -Contains "Svchost""

If the property value contains a single object, PowerShell converts it to a collection of one object.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`EQ`, `Indicates that this cmdlet gets objects if the property value is the same as the specified value.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`FilterScript`, `Specifies the script block that is used to filter the objects. Enclose the script block in braces ( {} ).

The parameter name, FilterScript , is optional.`, `ScriptBlock`),
        new Parameter(`GE`, `Indicates that this cmdlet gets objects if the property value is greater than or equal to the specified value.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`GT`, `Indicates that this cmdlet gets objects if the property value is greater than the specified value.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`In`, `Indicates that this cmdlet gets objects if the property value matches any of the specified values.

For example: "Get-Process | where -Property ProcessName -in -Value "Svchost", "TaskHost", "WsmProvHost""

If the value of the Value parameter is a single object, PowerShell converts it to a collection of one object.

If the property value of an object is an array, PowerShell uses reference equality to determine a match. "Where-Object" returns the object only if the value of the Property parameter and any value of Value are the same instance of an object.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`InputObject`, `Specifies the objects to be filtered. You can also pipe the objects to "Where-Object".

When you use the InputObject parameter with "Where-Object", instead of piping command results to "Where-Object", the InputObject value is treated as a single object. This is true even if the value is a collection that is the result of a command, such as "-InputObject (Get-Process)". Because InputObject cannot return individual properties from an array or collection of objects, we recommend that, if you use "Where-Object" to filter a collection of objects for those objects that have specific values in defined properties, you use "Where-Object" in the pipeline, as shown in the examples in this topic.`, `PSObject`),
        new Parameter(`Is`, `Indicates that this cmdlet gets objects if the property value is an instance of the specified .NET Framework type. Enclose the type name in square brackets.

For example, "Get-Process | where StartTime -Is [DateTime]"

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`IsNot`, `Indicates that this cmdlet gets objects if the property value is not an instance of the specified .NET Framework type.

For example, "Get-Process | where StartTime -IsNot [DateTime]"

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`LE`, `Indicates that this cmdlet gets objects if the property value is less than or equal to the specified value.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`LT`, `Indicates that this cmdlet gets objects if the property value is less than the specified value.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`Like`, `Indicates that this cmdlet gets objects if the property value matches a value that includes wildcard characters.

For example: "Get-Process | where ProcessName -Like "*host""

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`Match`, `Indicates that this cmdlet gets objects if the property value matches the specified regular expression. When the input is scalar, the matched value is saved in "$Matches" automatic variable.

For example: "Get-Process | where ProcessName -Match "shell""

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`NE`, `Indicates that this cmdlet gets objects if the property value is different than the specified value.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`NotContains`, `Indicates that this cmdlet gets objects if none of the items in the property value is an exact match for the specified value.

For example: "Get-Process | where ProcessName -NotContains "Svchost"" NotContains refers to a collection of values and is true if the collection does not contain any items that are an exact match for the specified value. If the input is a single object, PowerShell converts it to a collection of one object.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`NotIn`, `Indicates that this cmdlet gets objects if the property value is not an exact match for any of the specified values.

For example: "Get-Process | where -Value "svchost" -NotIn -Property ProcessName"

If the value of Value is a single object, PowerShell converts it to a collection of one object.

If the property value of an object is an array, PowerShell uses reference equality to determine a match. "Where-Object" returns the object only if the value of Property and any value of Value are not the same instance of an object.

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`NotLike`, `Indicates that this cmdlet gets objects if the property value does not match a value that includes wildcard characters.

For example: "Get-Process | where ProcessName -NotLike "*host""

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`NotMatch`, `Indicates that this cmdlet gets objects when the property value does not match the specified regular expression. When the input is scalar, the matched value is saved in "$Matches" automatic variable.

For example: "Get-Process | where ProcessName -NotMatch "PowerShell""

This parameter was introduced in Windows PowerShell 3.0.`, `SwitchParameter`),
        new Parameter(`Property`, `Specifies the name of an object property.

The parameter name, Property , is optional.

This parameter was introduced in Windows PowerShell 3.0.`, `String`),
        new Parameter(`Value`, `Specifies a property value.

The parameter name, Value , is optional.

This parameter was introduced in Windows PowerShell 3.0.`, `Object`),
    ], `Selects objects from a collection based on their property values.`, `Where-Object [-Property] <String> [[-Value] <Object>] -CContains [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CEQ [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CGE [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CGT [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CIn [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CLE [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CLT [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CLike [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CMatch [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CNE [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CNotContains [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CNotIn [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CNotLike [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -CNotMatch [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -Contains [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] [-EQ] [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-FilterScript] <ScriptBlock> [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -GE [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -GT [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] -In [-InputObject <PSObject>] [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] [-InputObject <PSObject>] -Is [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] [-InputObject <PSObject>] -IsNot [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] [-InputObject <PSObject>] -LE [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] [-InputObject <PSObject>] -LT [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] [-InputObject <PSObject>] -Like [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] [-InputObject <PSObject>] -Match [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] [-InputObject <PSObject>] -NE [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] [-InputObject <PSObject>] -NotContains [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] [-InputObject <PSObject>] -NotIn [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] [-InputObject <PSObject>] -NotLike [<CommonParameters>]

Where-Object [-Property] <String> [[-Value] <Object>] [-InputObject <PSObject>] -NotMatch [<CommonParameters>]`, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Write-Debug`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Write-Error`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Write-Host`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Write-Information`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Write-Output`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Write-Progress`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Write-Verbose`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
    new ConsoleCommand(`Write-Warning`, [], `See help file for details.`, ``, "", (parameters, paramDictionary) => {
    }),
];
export const PSCoreCommands = commands;
//# sourceMappingURL=PSCoreCommands.js.map