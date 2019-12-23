import { ConsoleCommand } from "../Models/ConsoleCommand.js";
import { Parameter } from "../Models/Parameter.js";
var commands = [
    new ConsoleCommand(`append`, [
        new Parameter(`[<Drive>:]<Path>`, `Specifies a drive and directory to append.`, ``),
        new Parameter(`/x:on`, `Applies appended directories to file searches and launching applications.`, ``),
        new Parameter(`/x:off`, `Applies appended directories only to requests to open files.</br>/x:off is the default setting.`, ``),
        new Parameter(`/path:on`, `Applies appended directories to file requests that already specify a path. /path:on is the default setting.`, ``),
        new Parameter(`/path:off`, `Turns off the effect of /path:on.`, ``),
        new Parameter(`/e`, `Stores a copy of the appended directory list in an environment variable named APPEND. /e may be used only the first time you use append after starting your system.`, ``),
        new Parameter(`;`, `Clears the appended directory list.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Allows programs to open data files in specified directories as if they were in the current directory. If used without parameters, append displays the appended directory list.`, `append [[<Drive>:]<Path>[;...]] [/x[:on|:off]] [/path:[:on|:off] [/e] 

append ;`, "", () => { }),
    new ConsoleCommand(`arp`, [
        new Parameter(`/a [<Inetaddr>] [/n <ifaceaddr>]`, `Displays current arp cache tables for all interfaces. The /n parameter is case-sensitive.<br /><br />To display the arp cache entry for a specific IP address, use arp /a with the *Inetaddr* parameter, where *Inetaddr* is an IP address. If *Inetaddr* is not specified, the first applicable interface is used.<br /><br />To display the arp cache table for a specific interface, use the /n*ifaceaddr* parameter in conjunction with the /a parameter where *ifaceaddr* is the IP address assigned to the interface.`, ``),
        new Parameter(`/g [<Inetaddr>] [/n <ifaceaddr>]`, `Identical to /a.`, ``),
        new Parameter(`[/d <Inetaddr> [<ifaceaddr>]`, `deletes an entry with a specific IP address, where *Inetaddr* is the IP address.<br /><br />To delete an entry in a table for a specific interface, use the *ifaceaddr* parameter where *ifaceaddr* is the IP address assigned to the interface.<br /><br />To delete all entries, use the asterisk (*) wildcard character in place of *Inetaddr*.`, ``),
        new Parameter(`/s <Inetaddr> <Etheraddr> [<ifaceaddr>]`, `adds a static entry to the arp cache that resolves the IP address *Inetaddr* to the physical address *Etheraddr*.<br /><br />To add a static arp cache entry to the table for a specific interface, use the *ifaceaddr* parameter where *ifaceaddr* is an IP address assigned to the interface.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays and modifies entries in the Address Resolution Protocol (ARP) cache. The ARP cache contains one or more tables that are used to store IP addresses and their resolved Ethernet or Token Ring physical addresses. There is a separate table for each Ethernet or Token Ring network adapter installed on your computer. Used without parameters, arp displays help information.`, `arp [/a [<Inetaddr>] [/n <ifaceaddr>]] [/g [<Inetaddr>] [-n <ifaceaddr>]] [/d <Inetaddr> [<ifaceaddr>]] [/s <Inetaddr> <Etheraddr> [<ifaceaddr>]]`, "", () => { }),
    new ConsoleCommand(`assoc`, [
        new Parameter(`<.ext>`, `Specifies the file name extension.`, ``),
        new Parameter(`<FileType>`, `Specifies the file type to associate with the specified file name extension.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays or modifies file name extension associations. If used without parameters, assoc displays a list of all the current file name extension associations.`, `assoc [<.ext>[=[<FileType>]]]`, "", () => { }),
    new ConsoleCommand(`at`, [
        new Parameter(`\\<computerName>`, `Specifies a remote computer. If you omit this parameter, at schedules the commands and programs on the local computer.`, ``),
        new Parameter(`<id>`, `Specifies the identification number assigned to a scheduled command.`, ``),
        new Parameter(`/delete`, `Cancels a scheduled command. If you omit *ID*, all of the scheduled commands on the computer are canceled.`, ``),
        new Parameter(`/yes`, `Answers yes to all queries from the system when you delete scheduled events.`, ``),
        new Parameter(`<time>`, `Specifies the time when you want to run the command. time is expressed as Hours:Minutes in 24-hour notation (that is, 00:00 [midnight] through 23:59).`, ``),
        new Parameter(`/interactive`, `Allows *command* to interact with the desktop of the user who is logged on at the time *Command* runs.`, ``),
        new Parameter(`/every:`, `Runs *command* on every specified day or days of the week or month (for example, every Thursday, or the third day of every month).`, ``),
        new Parameter(`<date>`, `Specifies the date when you want to run the command. You can specify one or more days of the week (that is, type M,T,W,Th,F,S,Su) or one or more days of the month (that is, type 1 through 31). Separate multiple date entries with commas. If you omit *date*, at uses the current day of the month.`, ``),
        new Parameter(`/next:`, `Runs *command* on the next occurrence of the day (for example, next Thursday).`, ``),
        new Parameter(`<command>`, `Specifies the Windows command, program (that is, .exe or .com file), or batch program (that is, .bat or .cmd file) that you want to run. When the command requires a path as an argument, use the absolute path (that is, the entire path beginning with the drive letter). If the command is on a remote computer, specify Universal Naming Convention (UNC) notation for the server and share name, rather than a remote drive letter.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Schedules commands and programs to run on a computer at a specified time and date. You can use at only when the Schedule service is running. Used without parameters, at lists scheduled commands.`, `at [\\computername] [[id] [/delete] | /delete [/yes]]

at [\\computername] <time> [/interactive] [/every:date[,...] | /next:date[,...]] <command>`, "", () => { }),
    new ConsoleCommand(`atmadm`, [
        new Parameter(`/c`, `Displays call information for all current connections to the atM network adapter installed on this computer.`, ``),
        new Parameter(`/a`, `Displays the registered atM network service access point (NSAP) address for each adapter installed in this computer.`, ``),
        new Parameter(`/s`, `Displays statistics for monitoring the status of active atM connections.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Monitors connections and addresses that are registered by the atM call Manager on an asynchronous transfer mode (atM) network. You can use atmadm to display statistics for incoming and outgoing calls on atM adapters. Used without parameters, atmadm displays statistics for monitoring the status of active atM connections. `, `atmadm [/c][/a][/s]`, "", () => { }),
    new ConsoleCommand(`attrib`, [
        new Parameter(`{+|-}r`, `Sets (+) or clears (-) the Read-only file attribute.`, ``),
        new Parameter(`{+|-}a`, `Sets (+) or clears (-) the Archive file attribute.`, ``),
        new Parameter(`{+|-}s`, `Sets (+) or clears (-) the System file attribute.`, ``),
        new Parameter(`{+|-}h`, `Sets (+) or clears (-) the Hidden file attribute.`, ``),
        new Parameter(`{+|-}i`, `Sets (+) or clears (-) the Not Content Indexed file attribute.`, ``),
        new Parameter(`[<Drive>:][<Path>][<FileName>]`, `Specifies the location and name of the directory, file, or group of files for which you want to display or change attributes. You can use the ? and * wildcard characters in the *FileName* parameter to display or change the attributes for a group of files.`, ``),
        new Parameter(`/s`, `Applies attrib and any command-line options to matching files in the current directory and all of its subdirectories.`, ``),
        new Parameter(`/d`, `Applies attrib and any command-line options to directories.`, ``),
        new Parameter(`/l`, `Applies attrib and any command-line options to the Symbolic Link, rather than the target of the Symbolic Link.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays, sets, or removes attributes assigned to files or directories. If used without parameters, attrib displays attributes of all files in the current directory.`, `attrib [{+|-}r] [{+|-}a] [{+|-}s] [{+|-}h] [{+|-}i] [<Drive>:][<Path>][<FileName>] [/s [/d] [/l]]`, "", () => { }),
    new ConsoleCommand(`auditpol`, [
        new Parameter(`/get`, `Displays the current audit policy.</br>See [Auditpol get](auditpol-get.md) for syntax and options.`, ``),
        new Parameter(`/set`, `Sets the audit policy.</br>See [Auditpol set](auditpol-set.md) for syntax and options.`, ``),
        new Parameter(`/list`, `Displays selectable policy elements.</br>See [Auditpol list](auditpol-list.md) for syntax and options.`, ``),
        new Parameter(`/backup`, `Saves the audit policy to a file.</br>See [Auditpol backup](auditpol-backup.md) for syntax and options.`, ``),
        new Parameter(`/restore`, `Restores the audit policy from a file that was previously created by using auditpol /backup.</br>See [Auditpol restore](auditpol-restore.md) for syntax and options.`, ``),
        new Parameter(`/clear`, `Clears the audit policy.</br>See [Auditpol clear](auditpol-clear.md) for syntax and options.`, ``),
        new Parameter(`/remove`, `Removes all per-user audit policy settings and disables all system audit policy settings.</br>See [Auditpol remove](auditpol-remove.md) for syntax and options.`, ``),
        new Parameter(`/resourceSACL`, `Configures global resource system access control lists (SACLs).</br>Note: Applies only to Windows 7 and Windows Server 2008 R2.</br>See [Auditpol resourceSACL](auditpol-resourcesacl.md).`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays information about and performs functions to manipulate audit policies.`, `Auditpol command [<sub-command><options>]`, "", () => { }),
    new ConsoleCommand(`autochk`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Runs when the computer is started and prior to Windows ServerÂ® 2008 R2 starting to verify the logical integrity of a file system.`, ``, "", () => { }),
    new ConsoleCommand(`autoconv`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `converts file allocation table (Fat) and Fat32 volumes to the NTFS file system, leaving existing files and directories intact at startup after autochk runs. volumes converted to the NTFS file system cannot be converted back to Fat or Fat32.`, ``, "", () => { }),
    new ConsoleCommand(`autofmt`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Formats a drive or partition when called from the Windows Recovery Console.`, ``, "", () => { }),
    new ConsoleCommand(`bcdboot`, [
        new Parameter(`source`, `Specifies the location of the Windows directory to use as the source for copying boot environment files.`, ``),
        new Parameter(`/l`, `Specifies the locale. The default locale is US English.`, ``),
        new Parameter(`/s`, `Specifies the volume letter of the system partition. The default is the system partition identified by the firmware.`, ``),
    ], `Enables you to quickly set up a system partition, or to repair the boot environment located on the system partition. The system partition is set up by copying a simple set of Boot Configuration Data (BCD) files to an existing empty partition.`, `bcdboot <source> [/l] [/s]`, "", () => { }),
    new ConsoleCommand(`bcdedit`, [
        new Parameter(`/?`, `Displays a list of BCDEdit commands. Running this command without an argument displays a summary of the available commands. To display detailed help for a particular command, run bcdedit /? <command>, where <command> is the name of the command you are searching for more information about. For example, bcdedit /? createstore displays detailed help for the Createstore command.`, ``),
    ], `Boot Configuration Data (BCD) files provide a store that is used to describe boot applications and boot application settings. The objects and elements in the store effectively replace Boot.ini.`, `BCDEdit /Command [<Argument1>] [<Argument2>] ...`, "", () => { }),
    new ConsoleCommand(`bdehdcfg`, [
        new Parameter(`[Bdehdcfg: driveinfo](bdehdcfg-driveinfo.md)`, `Displays the drive letter, the total size, the maximum free space, and the partition characteristics of the partitions on the drive specified. Only valid partitions are listed. Unallocated space is not listed if four primary or extended partitions already exist.`, ``),
        new Parameter(`[Bdehdcfg: target](bdehdcfg-target.md)`, `Defines which portion of a drive to use as the system drive and makes the portion active.`, ``),
        new Parameter(`[Bdehdcfg: newdriveletter](bdehdcfg-newdriveletter.md)`, `Assigns a new drive letter to the portion of a drive used as the system drive.`, ``),
        new Parameter(`[Bdehdcfg: size](bdehdcfg-size.md)`, `Determines the size of the system partition when a new system drive is being created.`, ``),
        new Parameter(`[Bdehdcfg: quiet](bdehdcfg-quiet.md)`, `Prevents the display of all actions and errors in the command-line interface and directs Bdehdcfg to use the "Yes" answer to any Yes/No prompts that may occur during subsequent drive preparation.`, ``),
        new Parameter(`[Bdehdcfg: restart](bdehdcfg-restart.md)`, `Directs the computer to restart after the drive preparation has finished.`, ``),
        new Parameter(`/?`, `Displays Help at the command prompt.`, ``),
    ], `Prepares a hard drive with the partitions necessary for BitLocker Drive Encryption. Most installations of Windows 7 will not need to use this tool because BitLocker setup includes the ability to prepare and repartition drives as required.`, `bdehdcfg [â€“driveinfo <DriveLetter>] [-target {default|unallocated|<DriveLetter> shrink|<DriveLetter> merge}] [â€“newdriveletter] [â€“size <SizeinMB>] [-quiet]`, "", () => { }),
    new ConsoleCommand(`bitsadmin`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `bitsadmin is a command-line tool that you can use to create download or upload jobs and monitor their progress.`, ``, "", () => { }),
    new ConsoleCommand(`bootcfg`, [
        new Parameter(`[bootcfg addsw](bootcfg-addsw.md)`, `adds operating system load options for a specified operating system entry.`, ``),
        new Parameter(`[bootcfg copy](bootcfg-copy.md)`, `Makes a copy of an existing boot entry, to which you can add command-line options.`, ``),
        new Parameter(`[bootcfg dbg1394](bootcfg-dbg1394.md)`, `Configures 1394 port debugging for a specified operating system entry.`, ``),
        new Parameter(`[bootcfg debug](bootcfg-debug.md)`, `adds or changes the debug settings for a specified operating system entry.`, ``),
        new Parameter(`[bootcfg default](bootcfg-default.md)`, `Specifies the operating system entry to designate as the default.`, ``),
        new Parameter(`[bootcfg delete](bootcfg-delete.md)`, `deletes an operating system entry in the [operating systems] section of the Boot.ini file.`, ``),
        new Parameter(`[bootcfg ems](bootcfg-ems.md)`, `Enables the user to add or change the settings for redirection of the Emergency Management Services console to a remote computer.`, ``),
        new Parameter(`[bootcfg query](bootcfg-query.md)`, `Queries and displays the [boot loader] and [operating systems] section entries from Boot.ini.`, ``),
        new Parameter(`[bootcfg raw](bootcfg-raw.md)`, `adds operating system load options specified as a string to an operating system entry in the [operating systems] section of the Boot.ini file.`, ``),
        new Parameter(`[bootcfg rmsw](bootcfg-rmsw.md)`, `removes operating system load options for a specified operating system entry.`, ``),
        new Parameter(`[bootcfg timeout](bootcfg-timeout.md)`, `changes the operating system time-out value.`, ``),
    ], `Configures, queries, or changes Boot.ini file settings.  `, `bootcfg <parameter> [arguments...]`, "", () => { }),
    new ConsoleCommand(`break`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Sets or clears extended CTRL+C checking on MS-DOS systems. If used without parameters, break displays the current setting.`, `break=[on|off]`, "", () => { }),
    new ConsoleCommand(`cacls`, [
        new Parameter(`<filename>`, `Required. Displays Acls of specified files.`, ``),
        new Parameter(`/t`, `changes Acls of specified files in the current directory and all subdirectories.`, ``),
        new Parameter(`/m`, `changes Acls of volumes mounted to a directory.`, ``),
        new Parameter(`/l`, `Work on the Symbolic Link itself versus the target.`, ``),
        new Parameter(`/s:sddl`, `replaces the Acls with those specified in the SDDL string (not valid with /e, /g, /r, /p, or /d).`, ``),
        new Parameter(`/e`, `edit ACL instead of replacing it.`, ``),
        new Parameter(`/c`, `Continue on access denied errors.`, ``),
        new Parameter(`/g user:<perm>`, `Grant specified user access rights.<br /><br />Valid values for permission:<br /><br />-   n - none<br />-   r - read<br />-   w - write<br />-   c - change (write)<br />-   f - full control`, ``),
        new Parameter(`/r user [...]`, `Revoke specified user's access rights (only valid with /e).`, ``),
        new Parameter(`[/p user:<perm> [...]`, `replace specified user's access rights.<br /><br />Valid values for permission:<br /><br />-   n - none<br />-   r - read<br />-   w - write<br />-   c - change (write)<br />-   f - full control`, ``),
        new Parameter(`[/d user [...]`, `Deny specified user access.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays or modifies discretionary access control lists (DACL) on specified files.  `, `cacls <filename> [/t] [/m] [/l] [/s[:sddl]] [/e] [/c] [/g user:<perm>] [/r user [...]] [/p user:<perm> [...]] [/d user [...]]`, "", () => { }),
    new ConsoleCommand(`call`, [
        new Parameter(`[<Drive>:][<Path>]<FileName>`, `Specifies the location and name of the batch program that you want to call. The *FileName* parameter is required, and it must have a .bat or .cmd extension.`, ``),
        new Parameter(`<BatchParameters>`, `Specifies any command-line information required by the batch program.`, ``),
        new Parameter(`:<Label>`, `Specifies the label that you want a batch program control to jump to.`, ``),
        new Parameter(`<Arguments>`, `Specifies the command-line information to be passed to the new instance of the batch program, beginning at *:Label.*`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Calls one batch program from another without stopping the parent batch program. The call command accepts labels as the target of the call.`, `call [Drive:][Path]<FileName> [<BatchParameters>] [:<Label> [<Arguments>]]`, "", () => { }),
    new ConsoleCommand(`cd`, [
        new Parameter(`/d`, `Changes the current drive as well as the current directory for a drive.`, ``),
        new Parameter(`<Drive>:`, `Specifies the drive to display or change (if different from the current drive).`, ``),
        new Parameter(`<Path>`, `Specifies the path to the directory that you want to display or change.`, ``),
        new Parameter(`[..]`, `Specifies that you want to change to the parent folder.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays the name of or changes the current directory. If used with only a drive letter (for example, "cd C:"), cd displays the names of the current directory in the specified drive. If used without parameters, cd displays the current drive and directory.`, `cd [/d] [<Drive>:][<Path>]

cd [..]

chdir [/d] [<Drive>:][<Path>]

chdir [..]`, "", () => { }),
    new ConsoleCommand(`certreq`, [
        new Parameter(`-Submit`, `Submits a request to a CA. For more information, see [Certreq -submit](#BKMK_Submit).`, ``),
        new Parameter(`-retrieve *RequestID*`, `Retrieves a response to a previous request from a CA. For more information, see [Certreq -retrieve](#BKMK_Retrieve).`, ``),
        new Parameter(`-New`, `Creates a new request from an .inf file. For more information, see [Certreq -new](#BKMK_New).`, ``),
        new Parameter(`-Accept`, `Accepts and installs a response to a certificate request. For more information, see [Certreq -accept](#BKMK_accept).`, ``),
        new Parameter(`-Policy`, `Sets the policy for a request. For more information, see [Certreq -policy](#BKMK_policy).`, ``),
        new Parameter(`-Sign`, `Signs a cross-certification or qualified subordination request. For more information, see [Certreq -sign](#BKMK_sign).`, ``),
        new Parameter(`-Enroll`, `Enrolls for or renews a certificate. For more information, see [Certreq -enroll](#BKMK_enroll).`, ``),
        new Parameter(`-?`, `Displays a list of certreq syntax, options, and descriptions.`, ``),
        new Parameter(`*<verb>* -?`, `Displays help for the verb specified.`, ``),
        new Parameter(`-v -?`, `Displays a verbose list of the certreq syntax, options, and descriptions.`, ``),
    ], `Certreq can be used to request certificates from a certification authority (CA), to retrieve a response to a previous request from a CA, to create a new request from an .inf file, to accept and install a response to a request, to construct a cross-certification or qualified subordination request from an existing CA certificate or request, and to sign a cross-certification or qualified subordination request.`, `CertReq [-Submit] [Options] [RequestFileIn [CertFileOut [CertChainFileOut [FullResponseFileOut]]]]`, "", () => { }),
    new ConsoleCommand(`certutil`, [
        new Parameter(`[-dump](#BKMK_dump)`, `Dump configuration information or files`, ``),
        new Parameter(`[-asn](#BKMK_asn)`, `Parse ASN.1 file`, ``),
        new Parameter(`[-decodehex](#BKMK_decodehex)`, `Decode hexadecimal-encoded file`, ``),
        new Parameter(`[-decode](#BKMK_decode)`, `Decode a Base64-encoded file`, ``),
        new Parameter(`[-encode](#BKMK_encode)`, `Encode a file to Base64`, ``),
        new Parameter(`[-deny](#BKMK_deny)`, `Deny a pending certificate request`, ``),
        new Parameter(`[-resubmit](#BKMK_resubmit)`, `Resubmit a pending certificate request`, ``),
        new Parameter(`[-setattributes](#BKMK_setattributes)`, `Set attributes for a pending certificate request`, ``),
        new Parameter(`[-setextension](#BKMK_setextension)`, `Set an extension for a pending certificate request`, ``),
        new Parameter(`[-revoke](#BKMK_revoke)`, `Revoke a certificate`, ``),
        new Parameter(`[-isvalid](#BKMK_isvalid)`, `Display the disposition of the current certificate`, ``),
        new Parameter(`[-getconfig](#BKMK_getconfig)`, `Get the default configuration string`, ``),
        new Parameter(`[-ping](#BKMK_ping)`, `Attempt to contact the Active Directory Certificate Services Request interface`, ``),
        new Parameter(`-pingadmin`, `Attempt to contact the Active Directory Certificate Services Admin interface`, ``),
        new Parameter(`[-CAInfo](#BKMK_CAInfo)`, `Display information about the certification authority`, ``),
        new Parameter(`[-ca.cert](#BKMK_ca.cert)`, `Retrieve the certificate for the certification authority`, ``),
        new Parameter(`[-ca.chain](#BKMK_ca.chain)`, `Retrieve the certificate chain for the certification authority`, ``),
        new Parameter(`[-GetCRL](#BKMK_GetCRL)`, `Get a certificate revocation list (CRL)`, ``),
        new Parameter(`[-CRL](#BKMK_CRL)`, `Publish new certificate revocation lists (CRLs) [or only delta CRLs]`, ``),
        new Parameter(`[-shutdown](#BKMK_shutdown)`, `Shutdown Active Directory Certificate Services`, ``),
        new Parameter(`[-installCert](#BKMK_installcert)`, `Install a certification authority certificate`, ``),
        new Parameter(`[-renewCert](#BKMK_renewcert)`, `Renew a certification authority certificate`, ``),
        new Parameter(`[-schema](#BKMK_schema)`, `Dump the schema for the certificate`, ``),
        new Parameter(`[-view](#BKMK_view)`, `Dump the certificate view`, ``),
        new Parameter(`[-db](#BKMK_db)`, `Dump the raw database`, ``),
        new Parameter(`[-deleterow](#BKMK_deleterow)`, `Delete a row from the server database`, ``),
        new Parameter(`[-backup](#BKMK_backup)`, `Backup Active Directory Certificate Services`, ``),
        new Parameter(`[-backupDB](#BKMK_backupDB)`, `Backup the Active Directory Certificate Services database`, ``),
        new Parameter(`[-backupKey](#BKMK_backupKey)`, `Backup the Active Directory Certificate Services certificate and private key`, ``),
        new Parameter(`[-restore](#BKMK_restore)`, `Restore Active Directory Certificate Services`, ``),
        new Parameter(`[-restoreDB](#BKMK_restoreDB)`, `Restore the Active Directory Certificate Services database`, ``),
        new Parameter(`[-restoreKey](#BKMK_restorekey)`, `Restore the Active Directory Certificate Services certificate and private key`, ``),
        new Parameter(`[-importPFX](#BKMK_importPFX)`, `Import certificate and private key`, ``),
        new Parameter(`[-dynamicfilelist](#BKMK_dynamicfilelist)`, `Display a dynamic file list`, ``),
        new Parameter(`[-databaselocations](#BKMK_databaselocations)`, `Display database locations`, ``),
        new Parameter(`[-hashfile](#BKMK_hashfile)`, `Generate and display a cryptographic hash over a file`, ``),
        new Parameter(`[-store](#BKMK_Store)`, `Dump the certificate store`, ``),
        new Parameter(`[-addstore](#BKMK_addstore)`, `Add a certificate to the store`, ``),
        new Parameter(`[-delstore](#BKMK_delstore)`, `Delete a certificate from the store`, ``),
        new Parameter(`[-verifystore](#BKMK_verifystore)`, `Verify a certificate in the store`, ``),
        new Parameter(`[-repairstore](#BKMK_repairstore)`, `Repair a key association or update certificate properties or the key security descriptor`, ``),
        new Parameter(`[-viewstore](#BKMK_viewstore)`, `Dump the certificates store`, ``),
        new Parameter(`[-viewdelstore](#BKMK_viewdelstore)`, `Delete a certificate from the store`, ``),
        new Parameter(`[-dsPublish](#BKMK_dsPublish)`, `Publish a certificate or certificate revocation list (CRL) to Active Directory`, ``),
        new Parameter(`[-ADTemplate](#BKMK_ADTemplate)`, `Display AD templates`, ``),
        new Parameter(`[-Template](#BKMK_template)`, `Display certificate templates`, ``),
        new Parameter(`[-TemplateCAs](#BKMK_TemplateCAs)`, `Display the certification authorities (CAs) for a certificate template`, ``),
        new Parameter(`[-CATemplates](#BKMK_CATemplates)`, `Display templates for CA`, ``),
        new Parameter(`[-SetCASites](#BKMK_SetCASites)`, `Manage Site Names for CAs`, ``),
        new Parameter(`[-enrollmentServerURL](#BKMK_enrollmentServerURL)`, `Display, add or delete enrollment server URLs associated with a CA`, ``),
        new Parameter(`[-ADCA](#BKMK_ADCA)`, `Display AD CAs`, ``),
        new Parameter(`[-CA](#BKMK_CA)`, `Display Enrollment Policy CAs`, ``),
        new Parameter(`[-Policy](#BKMK_Policy)`, `Display Enrollment Policy`, ``),
        new Parameter(`[-PolicyCache](#BKMK_PolicyCache)`, `Display or delete Enrollment Policy Cache entries`, ``),
        new Parameter(`[-CredStore](#BKMK_Credstore)`, `Display, add or delete Credential Store entries`, ``),
        new Parameter(`[-InstallDefaultTemplates](#BKMK_InstallDefaultTemplates)`, `Install default certificate templates`, ``),
        new Parameter(`[-URLCache](#BKMK_URLCache)`, `Display or delete URL cache entries`, ``),
        new Parameter(`[-pulse](#BKMK_pulse)`, `Pulse auto enrollment events`, ``),
        new Parameter(`[-MachineInfo](#BKMK_MachineInfo)`, `Display information about the Active Directory machine object`, ``),
        new Parameter(`[-DCInfo](#BKMK_DCInfo)`, `Display information about the domain controller`, ``),
        new Parameter(`[-EntInfo](#BKMK_EntInfo)`, `Display information about an enterprise CA`, ``),
        new Parameter(`[-TCAInfo](#BKMK_TCAInfo)`, `Display information about the CA`, ``),
        new Parameter(`[-SCInfo](#BKMK_SCInfo)`, `Display information about the smart card`, ``),
        new Parameter(`[-SCRoots](#BKMK_SCRoots)`, `Manage smart card root certificates`, ``),
        new Parameter(`[-verifykeys](#BKMK_verifykeys)`, `Verify a public or private key set`, ``),
        new Parameter(`[-verify](#BKMK_verify)`, `Verify a certificate, certificate revocation list (CRL), or certificate chain`, ``),
        new Parameter(`[-verifyCTL](#BKMK_verifyCTL)`, `Verify AuthRoot or Disallowed Certificates CTL`, ``),
        new Parameter(`[-sign](#BKMK_sign)`, `Re-sign a certificate revocation list (CRL) or certificate`, ``),
        new Parameter(`[-vroot](#BKMK_vroot)`, `Create or delete web virtual roots and file shares`, ``),
        new Parameter(`[-vocsproot](#BKMK_vocsproot)`, `Create or delete web virtual roots for an OCSP web proxy`, ``),
        new Parameter(`[-addEnrollmentServer](#BKMK_addEnrollmentServer)`, `Add an Enrollment Server application`, ``),
        new Parameter(`[-deleteEnrollmentServer](#BKMK_deleteEnrollmentServer)`, `Delete an Enrollment Server application`, ``),
        new Parameter(`[-addPolicyServer](#BKMK_addPolicyServer)`, `Add a Policy Server application`, ``),
        new Parameter(`[-deletePolicyServer](#BKMK_deletePolicyServer)`, `Delete a Policy Server application`, ``),
        new Parameter(`[-oid](#BKMK_oid)`, `Display the object identifier or set a display name`, ``),
        new Parameter(`[-error](#BKMK_error)`, `Display the message text associated with an error code`, ``),
        new Parameter(`[-getreg](#BKMK_getreg)`, `Display a registry value`, ``),
        new Parameter(`[-setreg](#BKMK_setreg)`, `Set a registry value`, ``),
        new Parameter(`[-delreg](#BKMK_delreg)`, `Delete a registry value`, ``),
        new Parameter(`[-ImportKMS](#BKMK_ImportKMS)`, `Import user keys and certificates into the server database for key archival`, ``),
        new Parameter(`[-ImportCert](#BKMK_ImportCert)`, `Import a certificate file into the database`, ``),
        new Parameter(`[-GetKey](#BKMK_GetKey)`, `Retrieve an archived private key recovery blob`, ``),
        new Parameter(`[-RecoverKey](#BKMK_RecoverKey)`, `Recover an archived private key`, ``),
        new Parameter(`[-MergePFX](#BKMK_MergePFX)`, `Merge PFX files`, ``),
        new Parameter(`[-ConvertEPF](#BKMK_ConvertEPF)`, `Convert a PFX file into an EPF file`, ``),
        new Parameter(`-?`, `Displays the list of verbs`, ``),
        new Parameter(`-*<verb>* -?`, `Displays help for the verb specified.`, ``),
        new Parameter(`-? -v`, `Displays a full list of verbs and`, ``),
    ], `Certutil.exe is a command-line program that is installed as part of Certificate Services. You can use Certutil.exe to dump and display certification authority (CA) configuration information, configure Certificate Services, backup and restore CA components, and verify certificates, key pairs, and certificate chains.`, `[Properties]

     19 = Empty ; Add archived property, OR:

     19 =       ; Remove archived property



     11 = "{text}Friendly Name" ; Add friendly name property



     127 = "{hex}" ; Add custom hexadecimal property

         _continue_ = "00 01 02 03 04 05 06 07 08 09 0a 0b 0c 0d 0e 0f"

         _continue_ = "10 11 12 13 14 15 16 17 18 19 1a 1b 1c 1d 1e 1f"



     2 = "{text}" ; Add Key Provider Information property

       _continue_ = "Container=Container Name&"

       _continue_ = "Provider=Microsoft Strong Cryptographic Provider&"

       _continue_ = "ProviderType=1&"

       _continue_ = "Flags=0&"

       _continue_ = "KeySpec=2"



     9 = "{text}" ; Add Enhanced Key Usage property

       _continue_ = "1.3.6.1.5.5.7.3.2,"

       _continue_ = "1.3.6.1.5.5.7.3.1,"`, "", () => { }),
    new ConsoleCommand(`change`, [
        new Parameter(`[change logon](change-logon.md)`, `Enables or disables logons from client sessions on an rd Session Host server, or displays current logon status.`, ``),
        new Parameter(`[change port](change-port.md)`, `lists or changes the COM port mappings to be compatible with MS-DOS applications.`, ``),
        new Parameter(`[change user](change-user.md)`, `changes the install mode for the rd Session Host server.`, ``),
    ], `changes Remote Desktop Session Host (rd Session Host) server settings for logons, COM port mappings, and install mode.`, `change logon

change port

change user`, "", () => { }),
    new ConsoleCommand(`chcp`, [
        new Parameter(`<NNN>`, `Specifies the code page.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Changes the active console code page. If used without parameters, chcp displays the number of the active console code page.`, `chcp [<NNN>]`, "", () => { }),
    new ConsoleCommand(`chdir`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `This command is the same as the cd command.  `, ``, "", () => { }),
    new ConsoleCommand(`chglogon`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Enables or disables logons from client sessions on an rd Session Host server, or displays current logon status.`, ``, "", () => { }),
    new ConsoleCommand(`chgport`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `lists or changes the COM port mappings to be compatible with MS-DOS applications.`, ``, "", () => { }),
    new ConsoleCommand(`chgusr`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
    ], `changes the install mode for the Remote Desktop Session Host (rd Session Host) server.  `, ``, "", () => { }),
    new ConsoleCommand(`chkdsk`, [
        new Parameter(`<Volume>`, `Specifies the drive letter (followed by a colon), mount point, or volume name.`, ``),
        new Parameter(`[<Path>]<FileName>`, `Use with file allocation table (FAT) and FAT32 only. Specifies the location and name of a file or set of files that you want chkdsk to check for fragmentation. You can use the ? and * wildcard characters to specify multiple files.`, ``),
        new Parameter(`/f`, `Fixes errors on the disk. The disk must be locked. If chkdsk cannot lock the drive, a message appears that asks you if you want to check the drive the next time you restart the computer.`, ``),
        new Parameter(`/v`, `Displays the name of each file in every directory as the disk is checked.`, ``),
        new Parameter(`/r`, `Locates bad sectors and recovers readable information. The disk must be locked. /r includes the functionality of /f, with the additional analysis of physical disk errors.`, ``),
        new Parameter(`/x`, `Forces the volume to dismount first, if necessary. All open handles to the drive are invalidated. /x also includes the functionality of /f.`, ``),
        new Parameter(`/i`, `Use with NTFS only. Performs a less vigorous check of index entries, which reduces the amount of time required to run chkdsk.`, ``),
        new Parameter(`/c`, `Use with NTFS only. Does not check cycles within the folder structure, which reduces the amount of time required to run chkdsk.`, ``),
        new Parameter(`/l[:<Size>]`, `Use with NTFS only. Changes the log file size to the size you type. If you omit the size parameter, /l displays the current size.`, ``),
        new Parameter(`/b`, `NTFS only: Clears the list of bad clusters on the volume and rescans all allocated and free clusters for errors. /b includes the functionality of /r. Use this parameter after imaging a volume to a new hard disk drive.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Checks the file system and file system metadata of a volume for logical and physical errors. If used without parameters, chkdsk displays only the status of the volume and does not fix any errors. If used with the /f, /r, /x, or /b parameters, it fixes errors on the volume.`, `chkdsk [<Volume>[[<Path>]<FileName>]] [/f] [/v] [/r] [/x] [/i] [/c] [/l[:<Size>]] [/b]`, "", () => { }),
    new ConsoleCommand(`chkntfs`, [
        new Parameter(`<Volume> [...]`, `Specifies one or more volumes to check when the computer starts. Valid volumes include drive letters (followed by a colon), mount points, or volume names.`, ``),
        new Parameter(`/d`, `Restores all chkntfs default settings, except the countdown time for automatic file checking. By default, all volumes are checked when the computer is started, and chkdsk runs on those that are dirty.`, ``),
        new Parameter(`/t [:<Time>]`, `Changes the Autochk.exe initiation countdown time to the amount of time specified in seconds. If you do not enter a time, /t displays the current countdown time.`, ``),
        new Parameter(`/x <Volume> [...]`, `Specifies one or more volumes to exclude from checking when the computer is started, even if the volume is marked as requiring chkdsk.`, ``),
        new Parameter(`/c <Volume> [...]`, `Schedules one or more volumes to be checked when the computer is started, and runs chkdsk on those that are dirty.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays or modifies automatic disk checking when the computer is started. If used without options, chkntfs displays the file system of the specified volume. If automatic file checking is scheduled to run, chkntfs displays whether the specified volume is dirty or is scheduled to be checked the next time the computer is started.`, `chkntfs <Volume> [...]

chkntfs [/d]

chkntfs [/t[:<Time>]]

chkntfs [/x <Volume> [...]]

chkntfs [/c <Volume> [...]]`, "", () => { }),
    new ConsoleCommand(`choice`, [
        new Parameter(`/c <Choice1><Choice2><â€¦>`, `Specifies the list of choices to be created. Valid choices include a-z, A-Z, 0-9, and extended ASCII characters (128-254). The default list is "YN", which is displayed as "[Y,N]?".`, ``),
        new Parameter(`/n`, `Hides the list of choices, although the choices are still enabled and the message text (if specified by /m) is still displayed.`, ``),
        new Parameter(`/cs`, `Specifies that the choices are case-sensitive. By default, the choices are not case-sensitive.`, ``),
        new Parameter(`/t <Timeout>`, `Specifies the number of seconds to pause before using the default choice specified by /d. Acceptable values are from 0 to 9999. If /t is set to 0, choice does not pause before returning the default choice.`, ``),
        new Parameter(`/d <Choice>`, `Specifies the default choice to use after waiting the number of seconds specified by /t. The default choice must be in the list of choices specified by /c.`, ``),
        new Parameter(`/m <"Text">`, `Specifies a message to display before the list of choices. If /m is not specified, only the choice prompt is displayed.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Prompts the user to select one item from a list of single-character choices in a batch program, and then returns the index of the selected choice. If used without parameters, choice displays the default choices Y and N.`, `choice [/c [<Choice1><Choice2><â€¦>]] [/n] [/cs] [/t <Timeout> /d <Choice>] [/m <"Text">]`, "", () => { }),
    new ConsoleCommand(`cipher`, [
        new Parameter(`/b`, `Aborts if an error is encountered. By default, cipher continues to run even if errors are encountered.`, ``),
        new Parameter(`/c`, `Displays information on the encrypted file.`, ``),
        new Parameter(`/d`, `Decrypts the specified files or directories.`, ``),
        new Parameter(`/e`, `Encrypts the specified files or directories. Directories are marked so that files that are added afterward will be encrypted.`, ``),
        new Parameter(`/h`, `Displays files with hidden or system attributes. By default, these files are not encrypted or decrypted.`, ``),
        new Parameter(`/k`, `Creates a new certificate and key for use with Encrypting File System (EFS) files. If the /k parameter is specified, all other parameters are ignored.`, ``),
        new Parameter(`/r:<FileName> [/smartcard]`, `Generates an EFS recovery agent key and certificate, then writes them to a .pfx file (containing certificate and private key) and  a .cer file (containing only the certificate). If /smartcard is specified, it writes the recovery key and certificate to a smart card, and no .pfx file is generated.`, ``),
        new Parameter(`/s:<Directory>`, `Performs the specified operation on all subdirectories in the specified *Directory*.`, ``),
        new Parameter(`/u [/n]`, `Finds all encrypted files on the local drive(s). If used with the /n parameter, no updates are made. If used without /n, /u compares the user's file encryption key or the recovery agent's key to the current ones, and updates them if they have changed. This parameter works only with /n.`, ``),
        new Parameter(`/w:<Directory>`, `Removes data from available unused disk space on the entire volume. If you use the /w parameter, all other parameters are ignored. The directory specified can be located anywhere in a local volume. If it is a mount point or points to a directory in another volume, the data on that volume is removed.`, ``),
        new Parameter(`/x[:efsfile] [<FileName>]`, `Backs up the EFS certificate and keys to the specified file name. If used with :efsfile, /x backs up the user's certificate(s) that were used to encrypt the file. Otherwise, the user's current EFS certificate and keys are backed up.`, ``),
        new Parameter(`/y`, `Displays your current EFS certificate thumbnail on the local computer.`, ``),
        new Parameter(`/adduser [/certhash:<Hash> `, ` /certfile:<FileName>]`, ``),
        new Parameter(`/rekey`, `Updates the specified encrypted file(s) to use the currently configured EFS key.`, ``),
        new Parameter(`/removeuser /certhash:<Hash>`, `Removes a user from the specified file(s). The *Hash* provided for /certhash must be the SHA1 hash of the certificate to remove.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays or alters the encryption of directories and files on NTFS volumes. If used without parameters, cipher displays the encryption state of the current directory and any files it contains.`, `cipher [/e | /d | /c] [/s:<Directory>] [/b] [/h] [PathName [...]]

cipher /k

cipher /r:<FileName> [/smartcard]

cipher /u [/n]

cipher /w:<Directory>

cipher /x[:efsfile] [FileName]

cipher /y

cipher /adduser [/certhash:<Hash> | /certfile:<FileName>] [/s:Directory] [/b] [/h] [PathName [...]]

cipher /removeuser /certhash:<Hash> [/s:<Directory>] [/b] [/h] [<PathName> [...]]

cipher /rekey [PathName [...]]`, "", () => { }),
    new ConsoleCommand(`clip`, [
        new Parameter(`<Command>`, `Specifies a command whose output you want to send to the Windows Clipboard.`, ``),
        new Parameter(`<FileName>`, `Specifies a file whose contents you want to send to the Windows Clipboard.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Redirects command output from the command line to the Windows clipboard. You can then paste this text output into other programs.`, `<Command> | clip

clip < <FileName>`, "", () => { }),
    new ConsoleCommand(`cls`, [
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Clears the Command Prompt window.`, `cls`, "", () => { }),
    new ConsoleCommand(`Cmd`, [
        new Parameter(`/c`, `Carries out the command specified by *String* and then stops.`, ``),
        new Parameter(`/k`, `Carries out the command specified by *String* and continues.`, ``),
        new Parameter(`/s`, `Modifies the treatment of *String* after /c or /k.`, ``),
        new Parameter(`/q`, `Turns the echo off.`, ``),
        new Parameter(`/d`, `Disables execution of AutoRun commands.`, ``),
        new Parameter(`/a`, `Formats internal command output to a pipe or a file as American National Standards Institute (ANSI).`, ``),
        new Parameter(`/u`, `Formats internal command output to a pipe or a file as Unicode.`, ``),
        new Parameter(`/t:{<B><F>|<F>}`, `Sets the background (*B*) and foreground (*F*) colors.`, ``),
        new Parameter(`/e:on`, `Enables command extensions.`, ``),
        new Parameter(`/e:off`, `Disables commands extensions.`, ``),
        new Parameter(`/f:on`, `Enables file and directory name completion.`, ``),
        new Parameter(`/f:off`, `Disables file and directory name completion.`, ``),
        new Parameter(`/v:on`, `Enables delayed environment variable expansion.`, ``),
        new Parameter(`/v:off`, `Disables delayed environment variable expansion.`, ``),
        new Parameter(`<String>`, `Specifies the command you want to carry out.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Starts a new instance of the command interpreter, Cmd.exe. If used without parameters, cmd displays the version and copyright information of the operating system.`, `cmd [/c|/k] [/s] [/q] [/d] [/a|/u] [/t:{<B><F>|<F>}] [/e:{on|off}] [/f:{on|off}] [/v:{on|off}] [<String>]`, "", () => { }),
    new ConsoleCommand(`cmdkey`, [
        new Parameter(`/add:<TargetName>`, `adds a user name and password to the list.<br /><br />Requires the parameter of <TargetName> which identifies the computer or domain name that this entry will be associated with.`, ``),
        new Parameter(`/generic:<TargetName>`, `adds generic credentials to the list.<br /><br />Requires the parameter of <TargetName> which identifies the computer or domain name that this entry will be associated with.`, ``),
        new Parameter(`/smartcard`, `Retrieves the credential from a smart card.`, ``),
        new Parameter(`/user:<UserName>`, `Specifies the user or account name to store with this entry. If *UserName* is not supplied, it will be requested.`, ``),
        new Parameter(`/pass:<Password>`, `Specifies the password to store with this entry. If *Password* is not supplied, it will be requested.`, ``),
        new Parameter(`/delete{:<TargetName> &#124; /ras}`, `deletes a user name and password from the list. If *TargetName* is specified, that entry will be deleted. If /ras is specified, the stored remote access entry will be deleted.`, ``),
        new Parameter(`/list:<TargetName>`, `Displays the list of stored user names and credentials. If *TargetName* is not specified, all stored user names and credentials will be listed.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `creates, lists, and deletes stored user names and passwords or credentials.`, `cmdkey [{/add:<TargetName>|/generic:<TargetName>}] {/smartcard|/user:<UserName> [/pass:<Password>]} [/delete{:<TargetName>|/ras}] /list:<TargetName>`, "", () => { }),
    new ConsoleCommand(`cmstp`, [
        new Parameter(`< ServiceProfileFileName >.exe`, `Specifies, by name, the installation package that contains the profile that you want to install.<br /><br />Required for Syntax 1 but not valid for Syntax 2.`, ``),
        new Parameter(`/q:a`, `Specifies that the profile should be installed without prompting the user. The verification message that the installation has succeeded will still appear.<br /><br />Required for Syntax 1 but not valid for Syntax 2.`, ``),
        new Parameter(`[Drive:][path] <ServiceProfileFileName>.inf`, `Required. Specifies, by name, the configuration file that determines how the profile should be installed.<br /><br />The [Drive:][path] parameter is not valid for Syntax 1.`, ``),
        new Parameter(`/nf`, `Specifies that the support files should not be installed.`, ``),
        new Parameter(`/ni`, `Specifies that a desktop icon should not be created. This parameter is only valid for computers running Windows 95, Windows 98, Windows NT 4.0, or Windows Millennium edition.`, ``),
        new Parameter(`/ns`, `Specifies that a desktop shortcut should not be created. This parameter is only valid for computers running a member of the Windows Server 2003 family, Windows 2000, or Windows XP.`, ``),
        new Parameter(`/s`, `Specifies that the service profile should be installed or uninstalled silently (without prompting for user response or displaying verification message).`, ``),
        new Parameter(`/su`, `Specifies that the service profile should be installed for a single user rather than for all users. This parameter is only valid for computers running a Windows Server 2003, Windows 2000, or Windows XP.`, ``),
        new Parameter(`/au`, `Specifies that the service profile should be installed for all users. This parameter is only valid for computers running Windows Server 2003, Windows 2000, or Windows XP.`, ``),
        new Parameter(`/u`, `Specifies that the service profile should be uninstalled.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Installs or removes a Connection Manager service profile. Used without optional parameters, cmstp installs a service profile with default settings appropriate to the operating system and to the user's permissions. `, `ServiceProfileFileName .exe /q:a /c:"cmstp.exe ServiceProfileFileName .inf [/nf] [/ni] [/ns] [/s] [/su] [/u]"`, "", () => { }),
    new ConsoleCommand(`color`, [
        new Parameter(`<B>`, `Specifies the background color.`, ``),
        new Parameter(`<F>`, `Specifies the foreground color.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Changes the foreground and background colors in the Command Prompt window for the current session. If used without parameters, color restores the default Command Prompt window foreground and background colors.`, `color [[<B>]<F>]`, "", () => { }),
    new ConsoleCommand(`comp`, [
        new Parameter(`<Data1>`, `Specifies the location and name of the first file or set of files that you want to compare. You can use wildcard characters (* and ?) to specify multiple files.`, ``),
        new Parameter(`<Data2>`, `Specifies the location and name of the second file or set of files that you want to compare. You can use wildcard characters (* and ?) to specify multiple files.`, ``),
        new Parameter(`/d`, `Displays differences in decimal format. (The default format is hexadecimal.)`, ``),
        new Parameter(`/a`, `Displays differences as characters.`, ``),
        new Parameter(`/l`, `Displays the number of the line where a difference occurs, instead of displaying the byte offset.`, ``),
        new Parameter(`/n=<Number>`, `Compares only the number of lines that are specified for each file, even if the files are different sizes.`, ``),
        new Parameter(`/c`, `Performs a comparison that is not case-sensitive.`, ``),
        new Parameter(`/off[line]`, `Processes files with the offline attribute set.`, ``),
        new Parameter(`/?`, `Displays Help at the command prompt.`, ``),
    ], `Compares the contents of two files or sets of files byte-by-byte. If used without parameters, comp prompts you to enter the files to compare.`, `comp [<Data1>] [<Data2>] [/d] [/a] [/l] [/n=<Number>] [/c]`, "", () => { }),
    new ConsoleCommand(`compact`, [
        new Parameter(`/c`, `Compresses the specified directory or file.`, ``),
        new Parameter(`/u`, `Uncompresses the specified directory or file.`, ``),
        new Parameter(`/s[:<Dir>]`, `Applies the compact command to all subdirectories of the specified directory (or of the current directory if none is specified).`, ``),
        new Parameter(`/a`, `Displays hidden or system files.`, ``),
        new Parameter(`/i`, `Ignores errors.`, ``),
        new Parameter(`/f`, `Forces compression or uncompression of the specified directory or file. /f is used in the case of a file that was partly compressed when the operation was interrupted by a system crash. To force the file to be compressed in its entirety, use the /c and /f parameters and specify the partially compressed file.`, ``),
        new Parameter(`/q`, `Reports only the most essential information.`, ``),
        new Parameter(`<FileName>`, `Specifies the file or directory. You can use multiple file names, and the * and ? wildcard characters.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays or alters the compression of files or directories on NTFS partitions. If used without parameters, compact displays the compression state of the current directory and the files it contains.`, `compact [/c | /u] [/s[:<Dir>]] [/a] [/i] [/f] [/q] [<FileName>[...]]`, "", () => { }),
    new ConsoleCommand(`convert`, [
        new Parameter(`<Volume>`, `Specifies the drive letter (followed by a colon), mount point, or volume name to convert to NTFS.`, ``),
        new Parameter(`/fs:ntfs`, `Required. Converts the volume to NTFS.`, ``),
        new Parameter(`/v`, `Runs convert in verbose mode, which displays all messages during the conversion process.`, ``),
        new Parameter(`/cvtarea:<FileName>`, `Specifies that the Master File Table (MFT) and other NTFS metadata files are written to an existing, contiguous placeholder file. This file must be in the root directory of the file system to be converted. Use of the /cvtarea parameter can result in a less fragmented file system after conversion. For best results, the size of this file should be 1 KB multiplied by the number of files and directories in the file system, although the convert utility accepts files of any size.</br>Important: You must create the placeholder file by using the fsutil file createnew command prior to running convert. Convert does not create this file for you. Convert overwrites this file with NTFS metadata. After conversion, any unused space in this file is freed.`, ``),
        new Parameter(`/nosecurity`, `Specifies that the security settings on the converted files and directories allow access by all users.`, ``),
        new Parameter(`/x`, `Dismounts the volume, if necessary, before it is converted. Any open handles to the volume will no longer be valid.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Converts file allocation table (FAT) and FAT32 volumes to the NTFS file system, leaving existing files and directories intact. Volumes converted to the NTFS file system cannot be converted back to FAT or FAT32.`, `convert [<Volume>] /fs:ntfs [/v] [/cvtarea:<FileName>] [/nosecurity] [/x]`, "", () => { }),
    new ConsoleCommand(`copy`, [
        new Parameter(`/d`, `Allows the encrypted files being copied to be saved as decrypted files at the destination.`, ``),
        new Parameter(`/v`, `Verifies that new files are written correctly.`, ``),
        new Parameter(`/n`, `Uses a short file name, if available, when copying a file with a name longer than eight characters, or with a file name extension longer than three characters.`, ``),
        new Parameter(`/y`, `Suppresses prompting to confirm that you want to overwrite an existing destination file.`, ``),
        new Parameter(`/-y`, `Prompts you to confirm that you want to overwrite an existing destination file.`, ``),
        new Parameter(`/z`, `Copies networked files in restartable mode.`, ``),
        new Parameter(`/a`, `Indicates an ASCII text file.`, ``),
        new Parameter(`/b`, `Indicates a binary file.`, ``),
        new Parameter(`<Source>`, `Required. Specifies the location from which you want to copy a file or set of files. *Source* can consist of a drive letter and colon, a directory name, a file name, or a combination of these.`, ``),
        new Parameter(`<Destination>`, `Required. Specifies the location to which you want to copy a file or set of files. *Destination* can consist of a drive letter and colon, a directory name, a file name, or a combination of these.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Copies one or more files from one location to another.`, `copy [/d] [/v] [/n] [/y | /-y] [/z] [/a | /b] <Source> [/a | /b] [+<Source> [/a | /b] [+ ...]] [<Destination> [/a | /b]]`, "", () => { }),
    new ConsoleCommand(`cprofile`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Cprofile - Cprofile is deprecated, and is not guaranteed to be supported in future releases of Windows.`, ``, "", () => { }),
    new ConsoleCommand(`cscript`, [
        new Parameter(`Scriptname.extension`, `Specifies the path and file name of the script file with optional file name extension.`, ``),
        new Parameter(`/B`, `Specifies batch mode, which does not display alerts, scripting errors, or input prompts.`, ``),
        new Parameter(`/D`, `Starts the debugger.`, ``),
        new Parameter(`/E:<Engine>`, `Specifies the engine that is used to run the script.`, ``),
        new Parameter(`/H:cscript`, `Registers cscript.exe as the default script host for running scripts.`, ``),
        new Parameter(`/H:wscript`, `Registers wscript.exe as the default script host for running scripts. This is the default.`, ``),
        new Parameter(`/I`, `Specifies interactive mode, which displays alerts, scripting errors, and input prompts. This is the default and the opposite of /B.`, ``),
        new Parameter(`/Job:<Identifier>`, `Runs the job identified by *Identifier* in a .wsf script file.`, ``),
        new Parameter(`/Logo`, `Specifies that the Windows Script Host banner is displayed in the console before the script runs. This is the default and the opposite of /Nologo.`, ``),
        new Parameter(`/Nologo`, `Specifies that the Windows Script Host banner is not displayed before the script runs.`, ``),
        new Parameter(`/S`, `Saves the current command-prompt options for the current user.`, ``),
        new Parameter(`/T:<Seconds>`, `Specifies the maximum time the script can run (in seconds). You can specify up to 32,767 seconds. The default is no time limit.`, ``),
        new Parameter(`/U`, `Specifies Unicode for input and output that is redirected from the console.`, ``),
        new Parameter(`/X`, `starts the script in the debugger.`, ``),
        new Parameter(`/?`, `Displays available command parameters and provides help for using them. This is the same as typing cscript.exe with no parameters and no script.`, ``),
        new Parameter(`ScriptArguments`, `Specifies the arguments passed to the script. Each script argument must be preceded by a slash (/).`, ``),
    ], `starts a script so that it runs in a command-line environment.`, `cscript <Scriptname.extension> [/B] [/D] [/E:<Engine>] [{/H:cscript|/H:wscript}] [/I] [/Job:<Identifier>] [{/Logo|/NoLogo}] [/S] [/T:<Seconds>] [/X] [/U] [/?] [<ScriptArguments>]`, "", () => { }),
    new ConsoleCommand(`date`, [
        new Parameter(`<Month-Day-Year>`, `Sets the date specified, where *Month* is the month (one or two digits), *Day* is the day (one or two digits), and *Year* is the year (two or four digits).`, ``),
        new Parameter(`/t`, `Displays the current date without prompting you for a new date.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays or sets the system date. If used without parameters, date displays the current system date setting and prompts you to enter a new date.`, `date [/t | <Month-Day-Year>]`, "", () => { }),
    new ConsoleCommand(`dcgpofix`, [
        new Parameter(`/ignoreschema`, `Ignores the version of the Active DirectoryÂ® schema mc</br>when you run this command. Otherwise, the command only works on the same schema version as the Windows version in which the command was shipped.`, ``),
        new Parameter(`/target {Domain `, ` DC `, ``),
        new Parameter(`/?`, `Displays Help at the command prompt.`, ``),
    ], `Recreates the default Group Policy Objects (GPOs) for a domain. For examples of how this command can be used, see [Examples](#BKMK_Examples).`, `DCGPOFix [/ignoreschema] [/target: {Domain | DC | Both}] [/?]`, "", () => { }),
    new ConsoleCommand(`defrag`, [
        new Parameter(`"<volume>"`, `Specifies the drive letter or mount point path of the volume to be defragmented or analyzed.`, ``),
        new Parameter(`A`, `Perform analysis on the specified volumes.`, ``),
        new Parameter(`C`, `Perform the operation on all volumes.`, ``),
        new Parameter(`D`, `Perform traditional defrag (this is the default). On a tiered volume though, traditional defrag is performed only on the Capacity tier.`, ``),
        new Parameter(`E`, `Perform the operation on all volumes except those specified.`, ``),
        new Parameter(`G`, `Optimize the storage tiers on the specified volumes.`, ``),
        new Parameter(`H`, `Run the operation at normal priority (default is low).`, ``),
        new Parameter(`I n`, `Tier optimization would run for at most n seconds on each volume.`, ``),
        new Parameter(`K`, `Perform slab consolidation on the specified volumes.`, ``),
        new Parameter(`L`, `Perform retrim on the specified volumes.`, ``),
        new Parameter(`M [n]`, `Run the operation on each volume in parallel in the background. At most n threads optimize the storage tiers in parallel.`, ``),
        new Parameter(`O`, `Perform the proper optimization for each media type.`, ``),
        new Parameter(`T`, `Track an operation already in progress on the specified volume.`, ``),
        new Parameter(`U`, `print the progress of the operation on the screen.`, ``),
        new Parameter(`V`, `print verbose output containing the fragmentation statistics.`, ``),
        new Parameter(`X`, `Perform free space consolidation on the specified volumes.`, ``),
        new Parameter(`?`, `Displays this help information.`, ``),
    ], `Locates and consolidates fragmented files on local volumes to improve system performance.`, `defrag <volumes> | /C | /E <volumes>    [/H] [/M [n]| [/U] [/V]]

defrag <volumes> | /C | /E <volumes> /A [/H] [/M [n]| [/U] [/V]]

defrag <volumes> | /C | /E <volumes> /X [/H] [/M [n]| [/U] [/V]]

defrag <volume> [/<Parameter>]*`, "", () => { }),
    new ConsoleCommand(`del`, [
        new Parameter(`<Names>`, `Specifies a list of one or more files or directories. Wildcards may be used to delete multiple files. If a directory is specified, all files within the directory will be deleted.`, ``),
        new Parameter(`/p`, `Prompts for confirmation before deleting the specified file.`, ``),
        new Parameter(`/f`, `Forces deletion of read-only files.`, ``),
        new Parameter(`/s`, `Deletes specified files from the current directory and all subdirectories. Displays the names of the files as they are being deleted.`, ``),
        new Parameter(`/q`, `Specifies quiet mode. You are not prompted for delete confirmation.`, ``),
        new Parameter(`/a[:]<Attributes>`, `Deletes files based on the following file attributes:</br>r Read-only files</br>h Hidden files</br>i Not content indexed files</br>s System files</br>a Files ready for archiving</br>l Reparse points</br>-  Prefix meaning 'not'`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Deletes one or more files. This command is the same as the erase command.`, `del [/p] [/f] [/s] [/q] [/a[:]<Attributes>] <Names>

erase [/p] [/f] [/s] [/q] [/a[:]<Attributes>] <Names>`, "", () => { }),
    new ConsoleCommand(`dfsrmig`, [
        new Parameter(`/SetGlobalState <state>`, `Sets the desired global migration state for the domain to the state that corresponds to the value specified by *state*.<br /><br />To proceed through the migration or the rollback processes, use this command to cycle through the valid states. This option enables you to initiate and control the migration process by setting the global migration state in AD DS on the PDC emulator. If the PDC emulator is not available, this command fails.<br /><br /> You can only set the global migration state to a stable state. The valid values for *state*, therefore, are 0 for the start state, 1 for the Prepared state, 2 for the Redirected state, and 3 for the Eliminated state.<br /><br />Migration to the Eliminated state is irreversible and rollback from that state is not possible, so use a value of 3 for *state* only when you are fully committd to using DFS Replication for SYSvol replication.`, ``),
        new Parameter(`/GetGlobalState`, `Retrieves the current global migration state for the domain from the local copy of the AD DS database, when run on the PDC emulator.<br /><br />Use this option to confirm that you set the correct global migration state. Only stable migration states can be global migration states, so the results that the dfsrmig command reports with the /GetGlobalState option correspond to the states you can set with the /SetGlobalState option.<br /><br />You should run the dfsrmig command with the /GetGlobalState option only on the PDC emulator. active directory replication replicates the global state to the other domain controllers in the domain, but replication latencies can cause inconsistencies if you run the dfsrmig command with the /GetGlobalState option on a domain controller other than the PDC emulator. To check the local migration status of a domain controller other than the PDC emulator, use the /GetMigrationState option instead.`, ``),
        new Parameter(`/GetMigrationState`, `Retrieves the current local migration state for all domain controllers in the domain, and determines whether those local states match the current global migration state.<br /><br />Use this option to determine if all domain controllers have reached the global migration state. The output of the dsfrmig command when you use the /GetMigrationState option indicates whether or not migration to the current global state is complete, and it lists the local migration state for any domain controllers that have not reached the current global migration state. Local migration state for domain controllers can include transition states for domain controllers that have not reached the current global migration state.`, ``),
        new Parameter(`/createGlobalObjects`, `creates the global objects and settings in AD DS that DFS Replication uses.<br /><br />You should not need to use this option during a normal migration process, because the DFS Replication service automatically creates these AD DS objects and settings during the migration from the start state to the Prepared state. Use this option to manually create these objects and settings in the following situations:<br /><br />-   A new read-only domain controller is promoted during migration. The DFS Replication service automatically creates the AD DS objects and settings for DFS Replication during the migration from the start state to the Prepared state. If a new read-only domain controller is promoted in the domain after this transition, but before migration to the Eliminated state, then the objects that correspond to the newly activated read-only domain controller are not created in AD DS causing replication and migration to fail.<br />-   In this case, you can run the dfsrmig command wth the /createGlobalObjects option to manually create the objects on any read-only domain controllers that do not already have them. Running this command does not affect the domain controllers that already have the objects and settings for the DFS Replication service.<br />-   The global settings for the DFS Replication service are missing or were deleted. If these settings are missing for a particular domain controller, migration from the start state to the Prepared state stalls at the Preparing transition state for the domain controller. In this case, you can use the dfsrmig command with the /createGlobalObjects option to manually create the settings. Note: Because the global AD DS settings for the DFS Replication service for a read-only domain controller are created on the PDC emulator, these settings need to replicate to the read-only domain controller from the PDC emulator before the DFS Replication service on the read-only domain controller can use these settings. Because of active irectory replication latencies, this replication can take some time to occur.`, ``),
        new Parameter(`/deleteRoNtfrsMember [<read_only_domain_controller_name>]`, `deletes the global AD DS settings for FRS replication that correspond to the specified read-only domain controller, or deletes the global AD DS settings for FRS replication for all read-only domain controllers if no value is specified for *read_only_domain_controller_name*.<br /><br />You should not need to use this option during a normal migration process, because the DFS Replication service automatically deletes these AD DS settings during the migration from the Redirected state to the Eliminated state. Because read-only domain controllers cannot delete these settings from AD DS, the PDC emulator performs this operation, and the changes eventually replicate to the read-only domain controllers after the applicable latencies for active directory replication.<br /><br />You use this option to manually delete the AD DS settings only when the automatic deletion fails on a read-only domain controller and stalls the read-only domain controller for a long ime during the migration from the Redirected state to the Eliminated state.`, ``),
        new Parameter(`/deleteRoDfsrMember [<read_only_domain_controller_name>]`, `deletes the global AD DS settings for DFS Replication that correspond to the specified read-only domain controller, or deletes the global AD DS settings for DFS Replication for all read-only domain controllers if no value is specified for *read_only_domain_controller_name*.<br /><br />Use this option to manually delete the AD DS settings only when the automatic deletion fails on a read-only domain controller and stalls the read-only domain controller for a long time when rolling back the migration from the Prepared state to the start state.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt. Equivalent to running dfsrmig without any options.`, ``),
    ], `The "dfsrmig" command migrates SYSvol replication from File Replication Service (FRS) to Distributed File System (DFS) Replication, provides information about the progress of the migration, and modifies active directory Domain Services (AD DS) objects to support the migration.`, `dfsrmig [/SetGlobalState <state> | /GetGlobalState | /GetMigrationState | /createGlobalObjects | 

/deleteRoNtfrsMember [<read_only_domain_controller_name>] | /deleteRoDfsrMember [<read_only_domain_controller_name>] | /?]`, "", () => { }),
    new ConsoleCommand(`diantz`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `This command is the same as the makecab command.`, ``, "", () => { }),
    new ConsoleCommand(`dir`, [
        new Parameter(`[<Drive>:][<Path>]`, `Specifies the drive and directory for which you want to see a listing.`, ``),
        new Parameter(`[<FileName>]`, `Specifies a particular file or group of files for which you want to see a listing.`, ``),
        new Parameter(`/p`, `Displays one screen of the listing at a time. To see the next screen, press any key on the keyboard.`, ``),
        new Parameter(`/q`, `Displays file ownership information.`, ``),
        new Parameter(`/w`, `Displays the listing in wide format, with as many as five file names or directory names on each line.`, ``),
        new Parameter(`/d`, `Displays the listing in the same format as /w, but the files are sorted by column.`, ``),
        new Parameter(`/a[[:]<Attributes>]`, `Displays only the names of those directories and files with the attributes that you specify. If you omit /a, dir displays the names of all files except hidden and system files. If you use /a without specifying *Attributes*, dir displays the names of all files, including hidden and system files.</br>The following list describes each of the values that you can use for *Attributes*. Using a colon (:) is optional. Use any combination of these values, and do not separate the values with spaces.</br>d Directories</br>h Hidden files</br>s System files</br>l Reparse points</br>r Read-only files</br>a Files ready for archiving</br>i Not content indexed files</br>- Prefix meaning "not"`, ``),
        new Parameter(`/o[[:]<SortOrder>]`, `Sorts the output according to *SortOrder*, which can be any combination of the following values:</br>n By name (alphabetical)</br>e By extension (alphabetical)</br>g Group directories first</br>s By size (smallest first)</br>d By date/time (oldest first)</br>- Prefix to reverse order</br>Note: Using a colon is optional. Multiple values are processed in the order in which you list them. Do not separate multiple values with spaces.</br>If *SortOrder* is not specified, dir /o lists the directories in alphabetic order, followed by the files, which are also sorted in alphabetic order.`, ``),
        new Parameter(`/t[[:]<TimeField>]`, `Specifies which time field to display or use for sorting. The following list describes each of the values you can use for *TimeField*:</br>c Creation</br>a Last access</br>w Last written`, ``),
        new Parameter(`/s`, `Lists every occurrence of the specified file name within the specified directory and all subdirectories.`, ``),
        new Parameter(`/b`, `Displays a bare list of directories and files, with no additional information. /b overrides /w.`, ``),
        new Parameter(`/l`, `Displays unsorted directory names and file names in lowercase.`, ``),
        new Parameter(`/n`, `Displays a long list format with file names on the far right of the screen.`, ``),
        new Parameter(`/x`, `Displays the short names generated for non-8dot3 file names. The display is the same as the display for /n, but the short name is inserted before the long name.`, ``),
        new Parameter(`/c`, `Displays the thousand separator in file sizes. This is the default behavior. Use /-c to hide separators.`, ``),
        new Parameter(`/4`, `Displays years in four-digit format.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays a list of a directory's files and subdirectories. If used without parameters, dir displays the disk's volume label and serial number, followed by a list of directories and files on the disk (including their names and the date and time each was last modified). For files, dir displays the name extension and the size in bytes. Dir also displays the total number of files and directories listed, their cumulative size, and the free space (in bytes) remaining on the disk.`, `dir [<Drive>:][<Path>][<FileName>] [...] [/p] [/q] [/w] [/d] [/a[[:]<Attributes>]][/o[[:]<SortOrder>]] [/t[[:]<TimeField>]] [/s] [/b] [/l] [/n] [/x] [/c] [/4]`, "", () => { }),
    new ConsoleCommand(`diskcomp`, [
        new Parameter(`<Drive1>`, `Specifies the drive containing one of the floppy disks.`, ``),
        new Parameter(`<Drive2>`, `Specifies the drive containing the other floppy disk.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Compares the contents of two floppy disks. If used without parameters, diskcomp uses the current drive to compare both disks.For examples of how to use this command, see [Examples](#BKMK_examples).`, `diskcomp [<Drive1>: [<Drive2>:]]`, "", () => { }),
    new ConsoleCommand(`diskcopy`, [
        new Parameter(`<Drive1>`, `Specifies the drive that contains the source disk.`, ``),
        new Parameter(`<Drive2>`, `Specifies the drive that contains the destination disk.`, ``),
        new Parameter(`/v`, `Verifies that the information is copied correctly. This option slows down the copying process.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Copies the contents of the floppy disk in the source drive to a formatted or unformatted floppy disk in the destination drive. If used without parameters, diskcopy uses the current drive for the source disk and the destination disk.`, `diskcopy [<Drive1>: [<Drive2>:]] [/v]`, "", () => { }),
    new ConsoleCommand(`diskperf`, [
        new Parameter(`-?`, `Displays context sensitive help.`, ``),
        new Parameter(`-Y`, `Start all disk performance counters when the computer restarts.`, ``),
        new Parameter(`-YD`, `Enable disk performance counters for physical drives when the computer restarts.`, ``),
        new Parameter(`-YV`, `Enable disk performance counters for logical drives or storage volumes when the computer restarts.`, ``),
        new Parameter(`-N`, `Disable all disk performance counters when the computer restarts.`, ``),
        new Parameter(`-ND`, `Disable disk performance counters for physical drives when the computer restarts.`, ``),
        new Parameter(`-NV`, `Disable disk performance counters for logical drives or storage volumes when the computer restarts.`, ``),
        new Parameter(`\\\*<computername>*`, `Specify the name of the computer where you want to enable or disable disk performance counters.`, ``),
    ], `In Windows 2000, physical and logical disk performance counters are not enabled by default.`, `diskperf [-Y[D|V] | -N[D|V]] [\\computername]`, "", () => { }),
    new ConsoleCommand(`diskraid`, [
        new Parameter(`FCR`, `Fast Crash Recovery Required`, ``),
        new Parameter(`FTL`, `Fault Tolerant`, ``),
        new Parameter(`MSR`, `Mostly Reads`, ``),
        new Parameter(`MXD`, `Maximum Drives`, ``),
        new Parameter(`MXS`, `Maximum Size Expected`, ``),
        new Parameter(`ORA`, `Optimal Read Alignment`, ``),
        new Parameter(`ORS`, `Optimal Read Size`, ``),
        new Parameter(`OSR`, `Optimize For Sequential Reads`, ``),
        new Parameter(`OSW`, `Optimize For Sequential Writes`, ``),
        new Parameter(`OWA`, `Optimal Write Alignment`, ``),
        new Parameter(`OWS`, `Optimal Write Size`, ``),
        new Parameter(`RBP`, `Rebuild Priority`, ``),
        new Parameter(`RBV`, `Read Back Verify Enabled`, ``),
        new Parameter(`RMP`, `Remap Enabled`, ``),
        new Parameter(`STS`, `Stripe Size`, ``),
        new Parameter(`WTC`, `Write-Through Caching Enabled`, ``),
        new Parameter(`YNK`, `Removable`, ``),
    ], `DiskRAID is a command-line tool that enables you to configure and manage redundant array of independent (or inexpensive) disks (RAID) storage subsystems.`, `add plex lun=n [noerr]

add tpgroup tportal=n [noerr]`, "", () => { }),
    new ConsoleCommand(`diskshadow`, [
        new Parameter(`[set_2](set_2.md)`, `Sets the context, options, verbose mode, and metadata file for creating shadow copies.`, ``),
        new Parameter(`[Simulate restore](simulate-restore.md)`, `Tests writer involvement in restore sessions on the computer without issuing PreRestore or PostRestore events to writers.`, ``),
        new Parameter(`[Load metadata](load-metadata.md)`, `Loads a metadata .cab file prior to importing a transportable shadow copy or loads the writer metadata in the case of a restore.`, ``),
        new Parameter(`[writer](writer.md)`, `verifies that a writer or component is included or excludes a writer or component from the backup or restore procedure.`, ``),
        new Parameter(`[add_1](add_1.md)`, `adds volumes to the set of volumes that are to be shadow copied, or adds aliases to the alias environment.`, ``),
        new Parameter(`[create_1](create_1.md)`, `starts the shadow copy creation process, using the current context and option settings.`, ``),
        new Parameter(`[exec](exec.md)`, `executes a file on the local computer.`, ``),
        new Parameter(`[Begin backup](begin-backup.md)`, `starts a full backup session.`, ``),
        new Parameter(`[End backup](end-backup.md)`, `Ends a full backup session and issues a Backupcomplete event with the appropriate writer state, if needed.`, ``),
        new Parameter(`[Begin restore](begin-restore.md)`, `starts a restore session and issues a PreRestore event to involved writers.`, ``),
        new Parameter(`[End restore](end-restore.md)`, `Ends a restore session and issues a PostRestore event to involved writers.`, ``),
        new Parameter(`[reset](reset.md)`, `resets diskshadow to the default state.`, ``),
        new Parameter(`[list](list.md)`, `lists writers, shadow copies, or currently registered shadow copy providers that are on the system.`, ``),
        new Parameter(`[delete shadows](delete-shadows.md)`, `deletes shadow copies.`, ``),
        new Parameter(`[import](import.md)`, `imports a transportable shadow copy from a loaded metadata file into the system.`, ``),
        new Parameter(`[mask](mask.md)`, `removes hardware shadow copies that were imported by using the import command.`, ``),
        new Parameter(`[expose](expose.md)`, `exposes a persistent shadow copy as a drive letter, share, or mount point.`, ``),
        new Parameter(`[unexpose](unexpose.md)`, `unexposes a shadow copy that was exposed by using the expose command.`, ``),
        new Parameter(`[break_2](break_2.md)`, `Disassociates a shadow copy volume from VSS.`, ``),
        new Parameter(`[revert](revert.md)`, `reverts a volume back to a specified shadow copy.`, ``),
        new Parameter(`[exit_1](exit_1.md)`, `exits diskshadow.`, ``),
    ], `diskshadow.exe is a tool that exposes the functionality offered by the volume shadow copy Service \(VSS\). By default, diskshadow uses an interactive command interpreter similar to that of diskraid or DiskPart. diskshadow also includes a scriptable mode.  `, `diskshadow`, "", () => { }),
    new ConsoleCommand(`dispdiag`, [
        new Parameter(`- testacpi`, `Runs hotkey diagnostics test. Displays the key name, code and scan code for any key pressed during the test.`, ``),
        new Parameter(`-d`, `Generates a dump file with test results.`, ``),
        new Parameter(`-delay <Seconds>`, `Delays the collection of data by specified time in *seconds*.`, ``),
        new Parameter(`-out <FilePath>`, `Specifies path and filename to save collected data. This must be the last parameter.`, ``),
        new Parameter(`-?`, `Displays available command parameters and provides help for using them.`, ``),
    ], `Logs display information to a file.`, `dispdiag [-testacpi] [-d] [-delay <Seconds>] [-out <FilePath>]`, "", () => { }),
    new ConsoleCommand(`Dnscmd`, [
        new Parameter(`<ServerName>`, `The IP address or host name of a remote or local DNS server.`, ``),
    ], `A command-line interface for managing DNS servers. This utility is useful in scripting batch files to help automate routine DNS management tasks, or to perform simple unattended setup and configuration of new DNS servers on your network.  `, `dnscmd <ServerName> <command> [<command parameters>]`, "", () => { }),
    new ConsoleCommand(`doskey`, [
        new Parameter(`/reinstall`, `Installs a new copy of Doskey.exe and clears the command history buffer.`, ``),
        new Parameter(`/listsize=<Size>`, `Specifies the maximum number of commands in the history buffer.`, ``),
        new Parameter(`/macros`, `Displays a list of all doskey macros. You can use the redirection symbol (>) with /macros to redirect the list to a file. You can abbreviate /macros to /m.`, ``),
        new Parameter(`/macros:all`, `Displays doskey macros for all executables.`, ``),
        new Parameter(`/macros:<ExeName>`, `Displays doskey macros for the executable specified by *ExeName*.`, ``),
        new Parameter(`/history`, `Displays all commands that are stored in memory. You can use the redirection symbol (>) with /history to redirect the list to a file. You can abbreviate /history as /h.`, ``),
        new Parameter(`[/insert `, ` /overstrike]`, ``),
        new Parameter(`/exename=<ExeName>`, `Specifies the program (that is, executable) in which the doskey macro runs.`, ``),
        new Parameter(`/macrofile=<FileName>`, `Specifies a file that contains the macros that you want to install.`, ``),
        new Parameter(`<MacroName>=[<Text>]`, `Creates a macro that carries out the commands specified by *Text*. *MacroName* specifies the name you want to assign to the macro. *Text* specifies the commands you want to record. If *Text* is left blank, *MacroName* is cleared of any assigned commands.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Calls Doskey.exe (which recalls previously entered command-line commands), edits command lines, and creates macros.`, `doskey [/reinstall] [/listsize=<Size>] [/macros:[all | <ExeName>] [/history] [/insert | /overstrike] [/exename=<ExeName>] [/macrofile=<FileName>] [<MacroName>=[<Text>]]`, "", () => { }),
    new ConsoleCommand(`driverquery`, [
        new Parameter(`/s <System>`, `Specifies the name or IP address of a remote computer. Do not use backslashes. The default is the local computer.`, ``),
        new Parameter(`/u [<Domain>]<Username>`, `Runs the command with the credentials of the user account as specified by *User* or *DomainUser*. By default, /s uses the credentials of the user who is currently logged on to the computer that is issuing the command. /u cannot be used unless /s is specified.`, ``),
        new Parameter(`/p <Password>`, `Specifies the password of the user account that is specified in the /u parameter. /p cannot be used unless /u is specified.`, ``),
        new Parameter(`/fo {table `, ` list `, ``),
        new Parameter(`/nh`, `Omits the header row from the displayed driver information. Not valid if the /fo parameter is set to list.`, ``),
        new Parameter(`/v`, `Displays verbose output. /v is not valid for signed drivers.`, ``),
        new Parameter(`/si`, `Provides information about signed drivers.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Enables an administrator to display a list of installed device drivers and their properties. If used without parameters, driverquery runs on the local computer.`, `driverquery [/s <System> [/u [<Domain>\]<Username> [/p <Password>]]] [/fo {table | list | csv}] [/nh] [/v | /si]`, "", () => { }),
    new ConsoleCommand(`echo`, [
        new Parameter(`[on | off]`, `Turns on or off the command echoing feature. Command echoing is on by default.`, ``),
        new Parameter(`<Message>`, `Specifies the text to display on the screen.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays messages or turns on or off the command echoing feature. If used without parameters, echo displays the current echo setting.`, `echo [<Message>]

echo [on | off]`, "", () => { }),
    new ConsoleCommand(`edit`, [
        new Parameter(`[<Drive>:][<Path>]<FileName> [<FileName2> [...]]`, `Specifies the location and name of one or more ASCII text files. If the file does not exist, MS-DOS Editor creates it. If the file exists, MS-DOS Editor opens it and displays its contents on the screen. *FileName* can contain wildcard characters (* and ?). Separate multiple file names with spaces.`, ``),
        new Parameter(`/b`, `Forces monochrome mode, so that MS-DOS Editor displays in black and white.`, ``),
        new Parameter(`/h`, `Displays the maximum number of lines possible for the current monitor.`, ``),
        new Parameter(`/r`, `Loads file(s) in read-only mode.`, ``),
        new Parameter(`/s`, `Forces the use of short filenames.`, ``),
        new Parameter(`<NNN>`, `Loads binary file(s), wrapping lines to *NNN* characters wide.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Starts MS-DOS Editor, which creates and changes ASCII text files.`, `edit [/b] [/h] [/r] [/s] [/<NNN>] [[<Drive>:][<Path>]<FileName> [<FileName2> [...]]`, "", () => { }),
    new ConsoleCommand(`endlocal`, [
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Ends localization of environment changes in a batch file, and restores environment variables to their values before the corresponding setlocal command was run.`, `endlocal`, "", () => { }),
    new ConsoleCommand(`erase`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `This command is the same as the del command. See [Del](del.md) for syntax and parameters.`, ``, "", () => { }),
    new ConsoleCommand(`eventcreate`, [
        new Parameter(`/s <Computer>`, `Specifies the name or IP address of a remote computer (do not use backslashes). The default is the local computer.`, ``),
        new Parameter(`/u <DomainUser>`, `Runs the command with the account permissions of the user specified by <User> or <DomainUser>. The default is the permissions of the current logged on user on the computer issuing the command.`, ``),
        new Parameter(`/p <Password>`, `Specifies the password of the user account that is specified in the /u parameter.`, ``),
        new Parameter(`/l {APPLICATION`, `SYSTEM}`, ``),
        new Parameter(`/so <SrcName>`, `Specifies the source to use for the event. A valid source can be any string and should represent the application or component that is generating the event.`, ``),
        new Parameter(`/t {ERROR`, `WARNING`, ``),
        new Parameter(`/id <EventID>`, `Specifies the event ID for the event. A valid ID is any number from 1 to 1000.`, ``),
        new Parameter(`/d <Description>`, `Specifies the description to use for the newly created event.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Enables an administrator to create a custom event in a specified event log. For examples of how to use this command, see [Examples](#BKMK_examples).`, `eventcreate [/s <Computer> [/u <Domain\User> [/p <Password>]] {[/l {APPLICATION|SYSTEM}]|[/so <SrcName>]} /t {ERROR|WARNING|INFORMATION|SUCCESSAUDIT|FAILUREAUDIT} /id <EventID> /d <Description>`, "", () => { }),
    new ConsoleCommand(`eventquery`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `eventquery is deprecated, and is not guaranteed to be supported in future releases of Windows.`, ``, "", () => { }),
    new ConsoleCommand(`eventtriggers`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Eventtriggers is deprecated, and is not guaranteed to be supported in future releases of Windows.`, ``, "", () => { }),
    new ConsoleCommand(`Evntcmd`, [
        new Parameter(`/s <computerName>`, `Specifies, by name, the computer on which you want to configure the translation of events to traps, trap destinations, or both. If you do not specify a computer, the configuration occurs on the local computer.`, ``),
        new Parameter(`/v <verbosityLevel>`, `Specifies which types of status messages appear as traps and trap destinations are configured. This parameter must be an integer between 0 and 10. If you specify 10, all types of messages appear, including tracing messages and warnings about whether trap configuration was successful. If you specify 0, no messages appear.`, ``),
        new Parameter(`/n`, `Specifies that the SNMP service should not be restarted if this computer receives trap configuration changes.`, ``),
        new Parameter(`<FileName>`, `Specifies, by name, the configuration file that contains information about the translation of events to traps and trap destinations you want to configure.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Configures the translation of events to traps, trap destinations, or both based on information in a configuration file.   `, `evntcmd [/s <computerName>] [/v <verbosityLevel>] [/n] <FileName>`, "", () => { }),
    new ConsoleCommand(`exit`, [
        new Parameter(`/b`, `exits the current batch script instead of exiting Cmd.exe. If executed from outside a batch script, exits Cmd.exe.`, ``),
        new Parameter(`<exitCode>`, `Specifies a numeric number. If /b is specified, the ERRORLEVEL environment variable is set to that number. If you are quitting Cmd.exe, the process exit code is set to that number.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `exits the Cmd.exe program (the command interpreter) or the current batch script.  `, `exit [/b] [<exitCode>]`, "", () => { }),
    new ConsoleCommand(`expand`, [
        new Parameter(`/r`, `renames expanded files.`, ``),
        new Parameter(`source`, `Specifies the files to expand. *Source* can consist of a drive letter and colon, a directory name, a file name, or a combination of these. You can use wildcards (* or ?).`, ``),
        new Parameter(`destination`, `Specifies where files are to be expanded.<br /><br />if *source* consists of multiple files and you do not specify /r, *destination* must be a directory.<br /><br />*Destination* can consist of a drive letter and colon, a directory name, a file name, or a combination of these.<br /><br />Destination file &#124; path specification.`, ``),
        new Parameter(`/i`, `renames expanded files but ignores the directory structure.<br /><br />This parameter applies to:  Windows Server 2008 R2  and  Windows 7 .`, ``),
        new Parameter(`/d`, `Displays a list of files in the source location. Does not expand or extract the files.`, ``),
        new Parameter(`/f:`, `Specifies the files in a cabinet (.cab) file that you want to expand. You can use wildcards (* or ?).`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `expands one or more compressed files. You can use this command to retrieve compressed files from distribution disks.  `, `expand [/r] <source> <destination>  

expand /r <source> [<destination>]  

expand /i <source> [<destination>]  

expand /d <source>.cab [/f:<files>]  

expand <source>.cab /f:<files> <destination>`, "", () => { }),
    new ConsoleCommand(`extract`, [
        new Parameter(`cabinet`, `File contains two or more files.`, ``),
        new Parameter(`filename`, `Name of the file to extract from the cabinet. Wild cards and multiple filenames (separated by blanks) may be used.`, ``),
        new Parameter(`source`, `Compressed file (a cabinet with only one file).`, ``),
        new Parameter(`newname`, `New filename to give the extracted file. If not supplied, the original name is used.`, ``),
        new Parameter(`/A`, `Process ALL cabinets. Follows cabinet chain starting in first cabinet mentioned.`, ``),
        new Parameter(`/C`, `Copy source file to destination (to copy from DMF disks).`, ``),
        new Parameter(`/D`, `Display cabinet directory (use with filename to avoid extract).`, ``),
        new Parameter(`/E`, `Extract (use instead of *.* to extract all files).`, ``),
        new Parameter(`/L dir`, `Location to place extracted files (default is current directory).`, ``),
        new Parameter(`/Y`, `Do not prompt before overwriting an existing file.`, ``),
    ], ``, `EXTRACT [/Y] [/A] [/D | /E] [/L dir] cabinet [filename ...]

EXTRACT [/Y] source [newname]

EXTRACT [/Y] /C source destination`, "", () => { }),
    new ConsoleCommand(`fc`, [
        new Parameter(`/a`, `Abbreviates the output of an ASCII comparison. Instead of displaying all of the lines that are different, fc displays only the first and last line for each set of differences.`, ``),
        new Parameter(`/b`, `Compares the two files in binary mode, byte by byte, and does not attempt to resynchronize the files after finding a mismatch. This is the default mode for comparing files that have the following file extensions: .exe, .com, .sys, .obj, .lib, or .bin.`, ``),
        new Parameter(`/c`, `Ignores the letter case.`, ``),
        new Parameter(`/l`, `Compares the files in ASCII mode, line-by-line, and attempts to resynchronize the files after finding a mismatch. This is the default mode for comparing files, except files with the following file extensions: .exe, .com, .sys, .obj, .lib, or .bin.`, ``),
        new Parameter(`/lb<N>`, `Sets the number of lines for the internal line buffer to *N*. The default length of the line buffer is 100 lines. If the files that you are comparing have more than 100 consecutive differing lines, fc cancels the comparison.`, ``),
        new Parameter(`/n`, `Displays the line numbers during an ASCII comparison.`, ``),
        new Parameter(`/off[line]`, `Does not skip files that have the offline attribute set.`, ``),
        new Parameter(`/t`, `Prevents fc from converting tabs to spaces. The default behavior is to treat tabs as spaces, with stops at each eighth character position.`, ``),
        new Parameter(`/u`, `Compares files as Unicode text files.`, ``),
        new Parameter(`/w`, `Compresses white space (that is, tabs and spaces) during the comparison. If a line contains many consecutive spaces or tabs, /w treats these characters as a single space. When used with /w, fc ignores white space at the beginning and end of a line.`, ``),
        new Parameter(`/<NNNN>`, `Specifies the number of consecutive lines that must match following a mismatch, before fc considers the files to be resynchronized. If the number of matching lines in the files is less than *NNNN*, fc displays the matching lines as differences. The default value is 2.`, ``),
        new Parameter(`[<Drive1>:][<Path1>]<FileName1>`, `Specifies the location and name of the first file or set of files to compare. *FileName1* is required.`, ``),
        new Parameter(`[<Drive2>:][<Path2>]<FileName2>`, `Specifies the location and name of the second file or set of files to compare. *FileName2* is required.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Compares two files or sets of files and displays the differences between them.`, `fc /a [/c] [/l] [/lb<N>] [/n] [/off[line]] [/t] [/u] [/w] [/<NNNN>] [<Drive1>:][<Path1>]<FileName1> [<Drive2>:][<Path2>]<FileName2>

fc /b [<Drive1:>][<Path1>]<FileName1> [<Drive2:>][<Path2>]<FileName2>`, "", () => { }),
    new ConsoleCommand(`find`, [
        new Parameter(`/v`, `Displays all lines that do not contain the specified <String>.`, ``),
        new Parameter(`/c`, `Counts the lines that contain the specified <String>and displays the total.`, ``),
        new Parameter(`/n`, `Precedes each line with the file's line number.`, ``),
        new Parameter(`/i`, `Specifies that the search is not case-sensitive.`, ``),
        new Parameter(`[/off[line]]`, `Does not skip files that have the offline attribute set.`, ``),
        new Parameter(`"<String>"`, `Required. Specifies the group of characters (enclosed in quotation marks) that you want to search for.`, ``),
        new Parameter(`[<Drive>:][<Path>]<FileName>`, `Specifies the location and name of the file in which to search for the specified string.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Searches for a string of text in a file or files, and displays lines of text that contain the specified string.`, `find [/v] [/c] [/n] [/i] [/off[line]] "<String>" [[<Drive>:][<Path>]<FileName>[...]]`, "", () => { }),
    new ConsoleCommand(`findstr`, [
        new Parameter(`/b`, `Matches the text pattern if it is at the beginning of a line.`, ``),
        new Parameter(`/e`, `Matches the text pattern if it is at the end of a line.`, ``),
        new Parameter(`/l`, `Processes search strings literally.`, ``),
        new Parameter(`/r`, `Processes search strings as regular expressions. This is the default setting.`, ``),
        new Parameter(`/s`, `Searches the current directory and all subdirectories.`, ``),
        new Parameter(`/i`, `Ignores the case of the characters when searching for the string.`, ``),
        new Parameter(`/x`, `Prints lines that match exactly.`, ``),
        new Parameter(`/v`, `Prints only lines that do not contain a match.`, ``),
        new Parameter(`/n`, `Prints the line number of each line that matches.`, ``),
        new Parameter(`/m`, `Prints only the file name if a file contains a match.`, ``),
        new Parameter(`/o`, `Prints character offset before each matching line.`, ``),
        new Parameter(`/p`, `Skips files with non-printable characters.`, ``),
        new Parameter(`/off[line]`, `Does not skip files that have the offline attribute set.`, ``),
        new Parameter(`/f:<File>`, `Gets a file list from the specified file.`, ``),
        new Parameter(`/c:<String>`, `Uses the specified text as a literal search string.`, ``),
        new Parameter(`/g:<File>`, `Gets search strings from the specified file.`, ``),
        new Parameter(`/d:<DirList>`, `Searches the specified list of directories. Each directory must be separated with a semicolon (;), for example "dir1;dir2;dir3".`, ``),
        new Parameter(`/a:<ColorAttribute>`, `Specifies color attributes with two hexadecimal digits. Type "color /?" for additional information.`, ``),
        new Parameter(`<Strings>`, `Specifies the text to search for in *FileName*. Required.`, ``),
        new Parameter(`[<Drive>:][<Path>]<FileName>[ ...]`, `Specifies the location and file or files to search. At least one file name is required.`, ``),
        new Parameter(`/?`, `Displays Help at the command prompt.`, ``),
    ], `Searches for patterns of text in files.`, `findstr [/b] [/e] [/l | /r] [/s] [/i] [/x] [/v] [/n] [/m] [/o] [/p] [/f:<File>] [/c:<String>] [/g:<File>] [/d:<DirList>] [/a:<ColorAttribute>] [/off[line]] <Strings> [<Drive>:][<Path>]<FileName>[ ...]`, "", () => { }),
    new ConsoleCommand(`finger`, [
        new Parameter(`-l`, `Displays user information in long list format.`, ``),
        new Parameter(`<User>`, `Specifies the user about which you want information. If you omit the *User* parameter, finger displays information about all users on the specified computer.`, ``),
        new Parameter(`@<Host>`, `Specifies the remote computer running the finger service where you are looking for user information. You can specify a computer name or IP address.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays information about a user or users on a specified remote computer (typically a computer running UNIX) that is running the finger service or daemon. The remote computer specifies the format and output of the user information display. Used without parameters, finger displays help. `, `finger [-l] [<User>] [@<Host>] [...]`, "", () => { }),
    new ConsoleCommand(`flattemp`, [
        new Parameter(`/query`, `Queries the current setting.`, ``),
        new Parameter(`/enable`, `Enables flat temporary folders. Users will share the temporary folder unless the temporary folder resides in the user s home folder.`, ``),
        new Parameter(`/disable`, `Disables flat temporary folders. Each user s temporary folder will reside in a separate folder (determined by the user s Session ID).`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Enables or disables flat temporary folders.`, `flattemp {/query | /enable | /disable}`, "", () => { }),
    new ConsoleCommand(`fondue`, [
        new Parameter(`/enable-feature:<*feature_name*>`, `Specifies the name of the Windows optional feature you want to enable. You can only enable one feature per command line. To enable multiple features, use fondue.exe for each feature.`, ``),
        new Parameter(`/caller-name:<*program_name*>`, `Specifies the program or process name when you call fondue.exe from a script or batch file. You can use this option to add the program name to the SQM report if there is an error.`, ``),
        new Parameter(`/hide-ux:{all &#124; rebootRequest}`, `Use all to hide all messages to the user including progress and permission requests to access Windows Update. If permission is required, the operation will fail.<br /><br />Use rebootRequest to only hide user messages asking for permission to reboot the computer. Use this option if you have a script that controls reboot requests.`, ``),
    ], `Enables Windows optional features by downloading required files from Windows Update or another source specified by Group Policy. The manifest file for the feature must already be installed in your Windows image. `, `fondue.exe /enable-feature:<feature_name> [/caller-name:<program_name>] [/hide-ux:{all | rebootRequest}]`, "", () => { }),
    new ConsoleCommand(`for`, [
        new Parameter(`{%%|%}<Variable>`, `Required. Represents a replaceable parameter. Use a single percent sign (%) to carry out the for command at the command prompt. Use double percent signs (%%) to carry out the for command within a batch file. Variables are case sensitive, and they must be represented with an alphabetical value such as %A, %B, or %C.`, ``),
        new Parameter(`(<Set>)`, `Required. Specifies one or more files, directories, or text strings, or a range of values on which to run the command. The parentheses are required.`, ``),
        new Parameter(`<Command>`, `Required. Specifies the command that you want to carry out on each file, directory, or text string, or on the range of values included in *Set*.`, ``),
        new Parameter(`<CommandLineOptions>`, `Specifies any command-line options that you want to use with the specified command.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Runs a specified command for each file in a set of files.`, `for {%%|%}<Variable> in (<Set>) do <Command> [<CommandLineOptions>]`, "", () => { }),
    new ConsoleCommand(`forfiles`, [
        new Parameter(`/p <Path>`, `Specifies the path from which to start the search. By default, searching starts in the current working directory.`, ``),
        new Parameter(`/m <SearchMask>`, `Searches files according to the specified search mask. The default search mask is *.*.`, ``),
        new Parameter(`/s`, `Instructs the forfiles command to search into subdirectories recursively.`, ``),
        new Parameter(`/c "<Command>"`, `Runs the specified command on each file. Command strings should be enclosed in quotation marks. The default command is "cmd /c echo @file".`, ``),
        new Parameter(`/d&nbsp;[{+|-}]&#8288;[{<Date>|&#8288;<Days>}]`, `Selects files with a last modified date within the specified time frame.</br>-   Selects files with a last modified date later than or equal to (+) or earlier than or equal to (-) the specified date, where *Date* is in the format MM/DD/YYYY.</br>-   Selects files with a last modified date later than or equal to (+) the current date plus the number of days specified, or earlier than or equal to (-) the current date minus the number of days specified.</br>-   Valid values for *Days* include any number in the range 0â€“32,768. If no sign is specified, + is used by default.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Selects and executes a command on a file or set of files. This command is useful for batch processing.`, `forfiles [/p <Path>] [/m <SearchMask>] [/s] [/c "<Command>"] [/d [{+|-}][{<Date>|<Days>}]]`, "", () => { }),
    new ConsoleCommand(`Format`, [
        new Parameter(`<Volume>`, `Specifies the mount point, volume name, or drive letter (followed by a colon) of the drive that you want to format. If you do not specify any of the following command-line options, format uses the volume type to determine the default format for the disk.`, ``),
        new Parameter(`/fs:{FAT`, `FAT32`, ``),
        new Parameter(`/v:<Label>`, `Specifies the volume label. If you omit the /v command-line option or use it without specifying a volume label, format prompts you for the volume label after the formatting is complete. Use the syntax /v: to prevent the prompt for a volume label. If you use a single format command to format more than one disk, all of the disks will be given the same volume label.`, ``),
        new Parameter(`/a:<UnitSize>`, `Specifies the allocation unit size to use on FAT, FAT32, or NTFS volumes. If you do not specify *UnitSize*, it is chosen based on volume size. Default settings are strongly recommended for general use. The following list presents valid values for NTFS, FAT, and FAT32 *UnitSize*:</br>512</br>1024</br>2048</br>4096</br>8192</br>16K</br>32K</br>64K</br>FAT and FAT32 also support 128K and 256K for a sector size greater than 512 bytes.`, ``),
        new Parameter(`/q`, `Performs a quick format. Deletes the file table and the root directory of a previously formatted volume, but does not perform a sector-by-sector scan for bad areas. You should use the /q command-line option to format only previously formatted volumes that you know are in good condition. Note that /q overrides /p.`, ``),
        new Parameter(`/f:<Size>`, `Specifies the size of the floppy disk to format. When possible, use this command-line option instead of the /t and /n command-line options. Windows accepts the following values for size:</br>-   1440 or 1440k or 1440kb</br>-   1.44 or 1.44m or 1.44mb</br>-   1.44-MB, double-sided, quadruple-density, 3.5-inch disk`, ``),
        new Parameter(`/t:<Tracks>`, `Specifies the number of tracks on the disk. When possible, use the /f command-line option instead. If you use the /t option, you must also use the /n option. These options together provide an alternative method of specifying the size of the disk that is being formatted. This option is not valid with the /f option.`, ``),
        new Parameter(`/n:<Sectors>`, `Specifies the number of sectors per track. When possible, use the /f command-line option instead of /n. If you use /n, you must also use /t. These two options together provide an alternative method of specifying the size of the disk that is being formatted. This option is not valid with the /f option.`, ``),
        new Parameter(`/p:<Passes>`, `Zeros every sector on the volume for the number of passes specified. This option is not valid with the /q option.`, ``),
        new Parameter(`/c`, `NTFS only. Files created on the new volume will be compressed by default.`, ``),
        new Parameter(`/x`, `Causes the volume to dismount, if necessary, before it is formatted. Any open handles to the volume will no longer be valid.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Formats a disk to accept Windows files.`, `format <Volume> [/fs:{FAT|FAT32|NTFS}] [/v:<Label>] [/q] [/a:<UnitSize>] [/c] [/x] [/p:<Passes>]

format <Volume> [/v:<Label>] [/q] [/f:<Size>] [/p:<Passes>]

format <Volume> [/v:<Label>] [/q] [/t:<Tracks> /n:<Sectors>] [/p:<Passes>]

format <Volume> [/v:<Label>] [/q] [/p:<Passes>]

format <Volume> [/q]`, "", () => { }),
    new ConsoleCommand(`freedisk`, [
        new Parameter(`/s <computer>`, `Specifies the name or IP address of a remote computer (do not use backslashes). The default is the local computer. This parameter applies to all files and folders specified in the command.`, ``),
        new Parameter(`/u [<Domain>\]<User>`, `Runs the script with the permissions of the specified user account. The default is system permissions.`, ``),
        new Parameter(`/p [<Password>]`, `Specifies the password of the user account that is specified in /u.`, ``),
        new Parameter(`/d <Drive>`, `Specifies the drive for which you want to find out the availability of free space. You must specify <Drive>for a remote computer.`, ``),
        new Parameter(`<Value>`, `Checks for a specific amount of free disk space. You can specify <Value>in bytes, KB, MB, GB, TB, PB, EB, ZB or YB.`, ``),
    ], `Checks to see if the specified amount of disk space is available before continuing with an installation process.`, `freedisk [/s <computer> [/u [<Domain>\]<User> [/p [<Password>]]]] [/d <Drive>] [<Value>]`, "", () => { }),
    new ConsoleCommand(`Fsutil`, [
        new Parameter(`[Fsutil 8dot3name](fsutil-8dot3name.md) `, ` Queries or changes the settings for short name behavior on the system, for example, generates 8.3 character-length file names. Removes short names for all files within a directory. Scans a directory and identifies registry keys that might be impacted if short names were stripped from the files in the directory.`, ``),
        new Parameter(`[Fsutil behavior](fsutil-behavior.md) `, `Queries or sets volume behavior.`, ``),
        new Parameter(`[Fsutil dirty](fsutil-dirty.md)`, ` Queries whether the volume's dirty bit is set or sets a volume's dirty bit. When a volume's dirty bit is set, autochk automatically checks the volume for errors the next time the computer is restarted.`, ``),
        new Parameter(`[Fsutil file](fsutil-file.md)`, `Finds a file by user name (if Disk Quotas are enabled), queries allocated ranges for a file, sets a file's short name, sets a file's valid data length, sets zero data for a file, creates a new file of a specified size, finds a file ID if given the name, or finds a file link name for a specified file ID.`, ``),
        new Parameter(`[Fsutil fsinfo](fsutil-fsinfo.md)`, `Lists all drives and queries the drive type, volume information, NTFS-specific volume information, or file system statistics.`, ``),
        new Parameter(`[Fsutil hardlink](fsutil-hardlink.md)`, `Lists hard links for a file, or creates a hard link (a directory entry for a file). Every file can be considered to have at least one hard link. On NTFS volumes, each file can have multiple hard links, so a single file can appear in many directories (or even in the same directory, with different names). Because all of the links reference the same file, programs can open any of the links and modify the file. A file is deleted from the file system only after all links to it are deleted. After you create a hard link, programs can use it like any other file name.`, ``),
        new Parameter(`[Fsutil objectid](fsutil-objectid.md)`, `Manages object identifiers, which are used by the Windows operating system to track objects such as files and directories.`, ``),
        new Parameter(`[Fsutil quota](fsutil-quota.md)`, `Manages disk quotas on NTFS volumes to provide more precise control of network-based storage. Disk quotas are implemented on a per-volume basis and enable both hard- and soft-storage limits to be implemented on a per-user basis.`, ``),
        new Parameter(`[Fsutil repair](fsutil-repair.md)`, `Queries or sets the self-healing state of the volume. Self-healing NTFS attempts to correct corruptions of the NTFS file system online without requiring Chkdsk.exe to be run. Includes initiating on-disk verification and waiting for repair completion.`, ``),
        new Parameter(`[Fsutil reparsepoint](fsutil-reparsepoint.md)`, `Queries or deletes reparse points (NTFS file system objects that have a definable attribute containing user-controlled data). Reparse points are used to extend functionality in the input/output (I/O) subsystem. They are used for directory junction points and volume mount points. They are also used by file system filter drivers to mark certain files as special to that driver.`, ``),
        new Parameter(`[Fsutil resource](fsutil-resource.md)`, `Creates a Secondary Transactional Resource Manager, starts or stops a Transactional Resource Manager, displays information about a Transactional Resource Manager  or modifies its behavior.`, ``),
        new Parameter(`[Fsutil sparse](fsutil-sparse.md)`, `Manages sparse files. A sparse file is a file with one or more regions of unallocated data in it. A program will see these unallocated regions as containing bytes with the value zero, but no disk space is used to represent these zeros. All meaningful or nonzero data is allocated, whereas all non-meaningful data (large strings of data composed of zeros) is not allocated. When a sparse file is read, allocated data is returned as stored and unallocated data is returned as zeros (by default in accordance with the C2 security requirement specification). Sparse file support allows data to be deallocated from anywhere in the file.`, ``),
        new Parameter(`[Fsutil tiering](fsutil-tiering.md)`, `Enables management of storage tier functions, such as setting and disabling flags and listing of tiers.`, ``),
        new Parameter(`[Fsutil transaction](fsutil-transaction.md)`, `Commits a specified transaction, rolls back a specified transaction, or displays info about the transaction.`, ``),
        new Parameter(`[Fsutil usn](fsutil-usn.md)`, `Manages the update sequence number (USN) change journal, which provides a persistent log of all changes made to files on the volume.`, ``),
        new Parameter(`[Fsutil volume](fsutil-volume.md)`, `Manages a volume. Dismounts a volume, queries to see how much free space is available on a disk, or finds a file that is using a specified cluster.`, ``),
        new Parameter(`[Fsutil wim](fsutil-wim.md)`, `Provides functions to discover and manage WIM-backed files.`, ``),
    ], `Performs tasks that are related to file allocation table (FAT) and NTFS file systems, such as managing reparse points, managing sparse files, or dismounting a volume. If it is used without parameters, Fsutil displays a list of supported subcommands. `, `> Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Windows-Subsystem-Linux`, "", () => { }),
    new ConsoleCommand(`ftype`, [
        new Parameter(`<FileType>`, `Specifies the file type to display or change.`, ``),
        new Parameter(`<OpenCommandString>`, `Specifies the open command string to use when opening files of the specified file type.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays or modifies file types that are used in file name extension associations. If used without an assignment operator (=), ftype displays the current open command string for the specified file type. If used without parameters, ftype displays the file types that have open command strings defined.`, `ftype [<FileType>[=[<OpenCommandString>]]]`, "", () => { }),
    new ConsoleCommand(`fveupdate`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `FveUpdate is an internally used tool that is run by setup when a computer is upgraded from Windows Vista or Windows Server 2008 to Windows 7 or Windows Server 2008 R2. It updates the metadata associated with BitLocker to the latest version. This tool cannot be run independently.`, ``, "", () => { }),
    new ConsoleCommand(`getmac`, [
        new Parameter(`/s <computer>`, `Specifies the name or IP address of a remote computer (do not use backslashes). The default is the local computer.`, ``),
        new Parameter(`/u <Domain>\<User>`, `Runs the command with the account permissions of the user specified by User or DomainUser. The default is the permissions of the current logged on user on the computer issuing the command.`, ``),
        new Parameter(`/p <Password>`, `Specifies the password of the user account that is specified in the /u parameter.`, ``),
        new Parameter(`/fo { TABLE &#124; list&#124; CSV}`, `Specifies the format to use for the query output. Valid values are TABLE, list, and CSV. The default format for output is TABLE.`, ``),
        new Parameter(`/nh`, `Suppresses column header in output. Valid when the /fo parameter is set to TABLE or CSV.`, ``),
        new Parameter(`/v`, `Specifies that the output display verbose information.`, ``),
        new Parameter(`/`, `?`, ``),
    ], `Returns the media access control (MAC) address and list of network protocols associated with each address for all network cards in each computer, either locally or across a network. `, `getmac[.exe][/s <computer> [/u <Domain\<User> [/p <Password>]]][/fo {TABLE | list | CSV}][/nh][/v]`, "", () => { }),
    new ConsoleCommand(`gettype`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Gettype is deprecated, and is not guaranteed to be supported in future releases of Windows.`, ``, "", () => { }),
    new ConsoleCommand(`goto`, [
        new Parameter(`<Label>`, `Specifies a text string that is used as a label in the batch program.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Directs cmd.exe to a labeled line in a batch program. Within a batch program, goto directs command processing to a line that is identified by a label. When the label is found, processing continues starting with the commands that begin on the next line.`, `goto <Label>`, "", () => { }),
    new ConsoleCommand(`gpfixup`, [
        new Parameter(`/v`, `Displays detailed status messages.</br>If this parameter is not used, only error messages or a summary status message of SUCCESS or FAILURE appears.`, ``),
        new Parameter(`/olddns:<OLDDNSNAME>`, `Specifies the old DNS name of the renamed domain as *<OLDDNSNAME>* when the domain rename operation changes the DNS name of a domain. You can use this parameter only if you also use the /newdns parameter to specify a new domain DNS name.`, ``),
        new Parameter(`/newdns:<NEWDNSNAME>`, `Specifies the new DNS name of the renamed domain as *<NEWDNSNAME>* when the domain rename operation changes the DNS name of a domain. You can use this parameter only if you also use the /olddns parameter to specify the old domain DNS name.`, ``),
        new Parameter(`/oldnb:<OLDFLATNAME>`, `Specifies the old NetBIOS name of the renamed domain as *<OLDFLATNAME>* when the domain rename operation changes the NetBIOS name of a domain. You can use this parameter only if you use the /newnb parameter to specify a new domain NetBIOS name.`, ``),
        new Parameter(`/newnb:<NEWFLATNAME>`, `Specifies the new NetBIOS name of the renamed domain as *<NEWFLATNAME>* when the domain rename operation changes the NetBIOS name of a domain. You can use this parameter only if you use the /oldnb parameter to specify the old domain NetBIOS name.`, ``),
        new Parameter(`/dc:<DCNAME>`, `Connect to the domain controller named *<DCNAME>* (a DNS name or a NetBIOS name). *<DCNAME>* must host a writable replica of the domain directory partition as indicated by one of the following:</br>-   The DNS name *<NEWDNSNAME>* by using /newdns</br>-   The NetBIOS name *<NEWFLATNAME>* by using /newnb</br>If this parameter is not used, connect to any domain controller in the renamed domain indicated by *<NEWDNSNAME>* or *<NEWFLATNAME>*.`, ``),
        new Parameter(`/sionly`, `Performs only the Group Policy fix that relates to managed software installation (the Software Installation extension for Group Policy). Skip the actions that fix Group Policy links and the SYSVOL paths in GPOs.`, ``),
        new Parameter(`/user:<USERNAME>`, `Runs this command in the security context of the user *<USERNAME>*, where *<USERNAME>* is in the format domainuser.</br>If this parameter is not used, runs this command as the logged in user.`, ``),
        new Parameter(`/pwd:{<PASSWORD>`, `*}`, ``),
        new Parameter(`/?`, `Displays Help at the command prompt.`, ``),
    ], `Fix domain name dependencies in Group Policy Objects and Group Policy links after a domain rename operation. For examples of how this command can be used, see [Examples](#BKMK_Examples).`, `Gpfixup [/v] 

[/olddns:<OLDDNSNAME> /newdns:<NEWDNSNAME>] 

[/oldnb:<OLDFLATNAME> /newnb:<NEWFLATNAME>] 

[/dc:<DCNAME>] [/sionly] 

[/user:<USERNAME> [/pwd:{<PASSWORD>|*}]] [/?]`, "", () => { }),
    new ConsoleCommand(`gpresult`, [
        new Parameter(`/s <system>`, `Specifies the name or IP address of a remote computer. Do not use backslashes. The default is the local computer.`, ``),
        new Parameter(`/u <USERNAME>`, `Uses the credentials of the specified user to run the command. The default user is the user who is logged on to the computer that issues the command.`, ``),
        new Parameter(`/p [<PASSWOrd>]`, `Specifies the password of the user account that is provided in the /u parameter. If /p is omitted, gpresult prompts for the password. /p cannot be used with /x or /h.`, ``),
        new Parameter(`/user [<TARGETDOMAIN>\]<TARGETUSER>`, `Specifies the remote user whose RSoP data is to be displayed.`, ``),
        new Parameter(`/scope {user &#124; computer}`, `Displays RSoP data for either the user or the computer. If /scope is omitted, gpresult displays RSoP data for both the user and the computer.`, ``),
        new Parameter(`[/x &#124; /h] <FILENAME>`, `Saves the report in either XML (/x) or HTML (/h) format at the location and with the file name that is specified by the *FILENAME* parameter. Cannot be used with /u, /p, /r, /v, or /z.`, ``),
        new Parameter(`/f`, `forces gpresult to overwrite the file name that is specified in the /x or /h option.`, ``),
        new Parameter(`/r`, `Displays RSoP summary data.`, ``),
        new Parameter(`/v`, `Displays verbose policy information. This includes detailed settings that were applied with a precedence of 1.`, ``),
        new Parameter(`/z`, `Displays all available information about Group Policy. This includes detailed settings that were applied with a precedence of 1 and higher.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays the Resultant Set of Policy (RSoP) information for a remote user and computer.`, `gpresult [/s <system> [/u <USERNAME> [/p [<PASSWOrd>]]]] [/user [<TARGETDOMAIN>\]<TARGETUSER>] [/scope {user | computer}] {/r | /v | /z | [/x | /h] <FILENAME> [/f] | /?}`, "", () => { }),
    new ConsoleCommand(`gpupdate`, [
        new Parameter(`/target:{Computer `, ` User}`, ``),
        new Parameter(`/force`, `Reapplies all policy settings. By default, only policy settings that have changed are applied.`, ``),
        new Parameter(`/wait:<VALUE>`, `Sets the number of seconds to wait for policy processing to finish before returning to the command prompt. When the time limit is exceeded, the command prompt appears, but policy processing continues. The default value is 600 seconds. The value 0 means not to wait. The value -1 means to wait indefinitely.</br>In a script, by using this command with a time limit specified, you can run gpupdate and continue with commands that do not depend upon the completion of gpupdate. Alternatively, you can use this command with no time limit specified to let gpupdate finish running before other commands that depend on it are run.`, ``),
        new Parameter(`/logoff`, `Causes a logoff after the Group Policy settings are updated. This is required for those Group Policy client-side extensions that do not process policy on a background update cycle but do process policy when a user logs on. Examples include user-targeted Software Installation and Folder Redirection. This option has no effect if there are no extensions called that require a logoff.`, ``),
        new Parameter(`/boot`, `Causes a computer restart after the Group Policy settings are applied. This is required for those Group Policy client-side extensions that do not process policy on a background update cycle but do process policy at computer startup. Examples include computer-targeted Software Installation. This option has no effect if there are no extensions called that require a restart.`, ``),
        new Parameter(`/sync`, `Causes the next foreground policy application to be done synchronously. Foreground policy is applied at computer boot and user logon. You can specify this for the user, computer, or both, by using the /target parameter. The /force and /wait parameters are ignored if you specify them.`, ``),
        new Parameter(`/?`, `Displays Help at the command prompt.`, ``),
    ], `Updates Group Policy settings. For examples of how this command can be used, see [Examples](#BKMK_Examples).`, `gpupdate [/target:{Computer | User}] [/force] [/wait:<VALUE>] [/logoff] [/boot] [/sync] [/?]`, "", () => { }),
    new ConsoleCommand(`graftabl`, [
        new Parameter(`<CodePage>`, `Specifies a code page to define the appearance of extended characters in graphics mode.</br>Valid code page identification numbers are:</br>437: United States</br>850: Multilingual (Latin I)</br>852: Slavic (Latin II)</br>855: Cyrillic (Russian)</br>857: Turkish</br>860: Portuguese</br>861: Icelandic</br>863: Canadian-French</br>865: Nordic</br>866: Russian</br>869: Modern Greek`, ``),
        new Parameter(`/status`, `Displays the current code page that graftabl is using.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Enables Windows operating systems to display an extended character set in graphics mode. If used without parameters, graftabl displays the previous and the current code page.`, `graftabl <CodePage>

graftabl /status`, "", () => { }),
    new ConsoleCommand(`help`, [
        new Parameter(`<Command>`, `Specifies the name of the command that you want information about.`, ``),
    ], `Provides online information about system commands (that is, non-network commands). If used without parameters, help lists and briefly describes every system command.`, `help [<Command>] 

[<Command>] /?`, "", () => { }),
    new ConsoleCommand(`helpctr`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Helpctr is deprecated, and is not guaranteed to be supported in future releases of Windows.`, ``, "", () => { }),
    new ConsoleCommand(`hostname`, [
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays the host name portion of the full computer name of the computer. `, `hostname`, "", () => { }),
    new ConsoleCommand(`icacls`, [
        new Parameter(`<FileName>`, `Specifies the file for which to display DACLs.`, ``),
        new Parameter(`<Directory>`, `Specifies the directory for which to display DACLs.`, ``),
        new Parameter(`/t`, `Performs the operation on all specified files in the current directory and its subdirectories.`, ``),
        new Parameter(`/c`, `Continues the operation despite any file errors. Error messages will still be displayed.`, ``),
        new Parameter(`/l`, `Performs the operation on a symbolic link versus its destination.`, ``),
        new Parameter(`/q`, `Suppresses success messages.`, ``),
        new Parameter(`[/save <ACLfile> [/t] [/c] [/l] [/q]]`, `Stores DACLs for all matching files into *ACLfile* for later use with /restore.`, ``),
        new Parameter(`[/setowner <Username> [/t] [/c] [/l] [/q]]`, `Changes the owner of all matching files to the specified user.`, ``),
        new Parameter(`[/findSID <Sid> [/t] [/c] [/l] [/q]]`, `Finds all matching files that contain a DACL explicitly mentioning the specified security identifier (SID).`, ``),
        new Parameter(`[/verify [/t] [/c] [/l] [/q]]`, `Finds all files with ACLs that are not canonical or have lengths inconsistent with ACE (access control entry) counts.`, ``),
        new Parameter(`[/reset [/t] [/c] [/l] [/q]]`, `Replaces ACLs with default inherited ACLs for all matching files.`, ``),
        new Parameter(`[/grant[:r] <Sid>:<Perm>[...]]`, `Grants specified user access rights. Permissions replace previously granted explicit permissions.</br>Without :r, permissions are added to any previously granted explicit permissions.`, ``),
        new Parameter(`[/deny <Sid>:<Perm>[...]]`, `Explicitly denies specified user access rights. An explicit deny ACE is added for the stated permissions and the same permissions in any explicit grant are removed.`, ``),
        new Parameter(`[/remove[:g|:d]] <Sid>[...]] [/t] [/c] [/l] [/q]`, `Removes all occurrences of the specified SID from the DACL.</br>:g removes all occurrences of granted rights to the specified SID.</br>:d removes all occurrences of denied rights to the specified SID.`, ``),
        new Parameter(`[/setintegritylevel [(CI)(OI)]<Level>:<Policy>[...]]`, `Explicitly adds an integrity ACE to all matching files. *Level* is specified as:</br>-   L[ow]</br>-   M[edium]</br>-   H[igh]</br>Inheritance options for the integrity ACE may precede the level and are applied only to directories.`, ``),
        new Parameter(`[/substitute <SidOld> <SidNew> [...]]`, `Replaces an existing SID (*SidOld*) with a new SID (*SidNew*). Requires the *Directory* parameter.`, ``),
        new Parameter(`/restore <ACLfile> [/c] [/l] [/q]`, `Applies stored DACLs from *ACLfile* to files in the specified directory. Requires the *Directory* parameter.`, ``),
        new Parameter(`/inheritancelevel:[e|d|r]`, `Sets the inheritance level: <br>  e - Enables enheritance <br>d - Disables inheritance and copies the ACEs <br>r - Removes all inherited ACEs`, ``),
    ], `Displays or modifies discretionary access control lists (DACLs) on specified files, and applies stored DACLs to files in specified directories.`, `icacls <FileName> [/grant[:r] <Sid>:<Perm>[...]] [/deny <Sid>:<Perm>[...]] [/remove[:g|:d]] <Sid>[...]] [/t] [/c] [/l] [/q] [/setintegritylevel <Level>:<Policy>[...]]

icacls <Directory> [/substitute <SidOld> <SidNew> [...]] [/restore <ACLfile> [/c] [/l] [/q]]`, "", () => { }),
    new ConsoleCommand(`if`, [
        new Parameter(`not`, `Specifies that the command should be carried out only if the condition is false.`, ``),
        new Parameter(`errorlevel <Number>`, `Specifies a true condition only if the previous program run by Cmd.exe returned an exit code equal to or greater than *Number*.`, ``),
        new Parameter(`<Command>`, `Specifies the command that should be carried out if the preceding condition is met.`, ``),
        new Parameter(`<String1>==<String2>`, `Specifies a true condition only if *String1* and *String2* are the same. These values can be literal strings or batch variables (for example, %1). You do not need to enclose literal strings in quotation marks.`, ``),
        new Parameter(`exist <FileName>`, `Specifies a true condition if the specified file name exists.`, ``),
        new Parameter(`<CompareOp>`, `Specifies a three-letter comparison operator. The following list represents valid values for *CompareOp*:</br>EQU Equal to</br>NEQ Not equal to</br>LSS Less than</br>LEQ Less than or equal to</br>GTR Greater than</br>GEQ Greater than or equal to`, ``),
        new Parameter(`/i`, `Forces string comparisons to ignore case.  You can use /i on the *String1*==*String2* form of if. These comparisons are generic, in that if both *String1* and *String2* are comprised of numeric digits only, the strings are converted to numbers and a numeric comparison is performed.`, ``),
        new Parameter(`cmdextversion <Number>`, `Specifies a true condition only if the internal version number associated with the command extensions feature of Cmd.exe is equal to or greater than the number specified. The first version is 1. It increases by increments of one when significant enhancements are added to the command extensions. The cmdextversion conditional is never true when command extensions are disabled (by default, command extensions are enabled).`, ``),
        new Parameter(`defined <Variable>`, `Specifies a true condition if *Variable* is defined.`, ``),
        new Parameter(`<Expression>`, `Specifies a command-line command and any parameters to be passed to the command in an else clause.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Performs conditional processing in batch programs.`, `if [not] ERRORLEVEL <Number> <Command> [else <Expression>]

if [not] <String1>==<String2> <Command> [else <Expression>]

if [not] exist <FileName> <Command> [else <Expression>]`, "", () => { }),
    new ConsoleCommand(`inuse`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
    ], `Inuse is deprecated, and is not guaranteed to be supported in future releases of Windows.`, ``, "", () => { }),
    new ConsoleCommand(`ipconfig`, [
        new Parameter(`/all`, `Displays the full TCP/IP configuration for all adapters. Adapters can represent physical interfaces, such as installed network adapters, or logical interfaces, such as dial-up connections.`, ``),
        new Parameter(`/allcompartments`, `Displays the full TCP/IP configuration for all compartments.`, ``),
        new Parameter(`/displaydns`, `Displays the contents of the DNS client resolver cache, which includes both entries preloaded from the local Hosts file and any recently obtained resource records for name queries resolved by the computer. The DNS Client service uses this information to resolve frequently queried names quickly, before querying its configured DNS servers.`, ``),
        new Parameter(`/flushdns`, `Flushes and resets the contents of the DNS client resolver cache. During DNS troubleshooting, you can use this procedure to discard negative cache entries from the cache, as well as any other entries that have been added dynamically.`, ``),
        new Parameter(`/registerdns`, `Initiates manual dynamic registration for the DNS names and IP addresses that are configured at a computer. You can use this parameter to troubleshoot a failed DNS name registration or resolve a dynamic update problem between a client and the DNS server without rebooting the client computer. The DNS settings in the advanced properties of the TCP/IP protocol determine which names are registered in DNS.`, ``),
        new Parameter(`/release [<Adapter>]`, `Sends a DHCPRELEASE message to the DHCP server to release the current DHCP configuration and discard the IP address configuration for either all adapters (if an adapter is not specified) or for a specific adapter if the *Adapter* parameter is included. This parameter disables TCP/IP for adapters configured to obtain an IP address automatically. To specify an adapter name, type the adapter name that appears when you use ipconfig without parameters.`, ``),
        new Parameter(`/release6[<Adapter>]`, `Sends a DHCPRELEASE message to the DHCPv6 server to release the current DHCP configuration and discard the IPv6 address configuration for either all adapters (if an adapter is not specified) or for a specific adapter if the *Adapter* parameter is included. This parameter disables TCP/IP for adapters configured to obtain an IP address automatically. To specify an adapter name, type the adapter name that appears when you use ipconfig without parameters.`, ``),
        new Parameter(`/renew [<Adapter>]`, `Renews DHCP configuration for all adapters (if an adapter is not specified) or for a specific adapter if the *Adapter* parameter is included. This parameter is available only on computers with adapters that are configured to obtain an IP address automatically. To specify an adapter name, type the adapter name that appears when you use ipconfig without parameters.`, ``),
        new Parameter(`/renew6 [<Adapter>]`, `Renews DHCPv6 configuration for all adapters (if an adapter is not specified) or for a specific adapter if the *Adapter* parameter is included. This parameter is available only on computers with adapters that are configured to obtain an IPv6 address automatically. To specify an adapter name, type the adapter name that appears when you use ipconfig without parameters.`, ``),
        new Parameter(`/setclassid <Adapter>[ <ClassID>]`, `Configures the DHCP class ID for a specified adapter. To set the DHCP class ID for all adapters, use the asterisk (*) wildcard character in place of *Adapter*. This parameter is available only on computers with adapters that are configured to obtain an IP address automatically. If a DHCP class ID is not specified, the current class ID is removed.`, ``),
        new Parameter(`/showclassid <Adapter>`, `Displays the DHCP class ID for a specified adapter. To see the DHCP class ID for all adapters, use the asterisk (*) wildcard character in place of *Adapter*. This parameter is available only on computers with adapters that are configured to obtain an IP address automatically.`, ``),
        new Parameter(`/?`, `Displays Help at the command prompt.`, ``),
    ], `Displays all current TCP/IP network configuration values and refreshes Dynamic Host Configuration Protocol (DHCP) and Domain Name System (DNS) settings. Used without parameters, ipconfig displays Internet Protocol version 4 (IPv4) and IPv6 addresses, subnet mask, and default gateway for all adapters.`, `ipconfig [/allcompartments] [/all] [/renew [<Adapter>]] [/release [<Adapter>]] [/renew6[<Adapter>]] [/release6 [<Adapter>]] [/flushdns] [/displaydns] [/registerdns] [/showclassid <Adapter>] [/setclassid <Adapter> [<ClassID>]]`, "", () => { }),
    new ConsoleCommand(`ipxroute`, [
        new Parameter(`servers[ /type=X]`, `Displays the Service Access Point (SAP) table for the specified server type.  X must be an integer. For example, /type=4 displays all file servers. If you do not specify /type, ipxroute servers displays all types of servers, listing them by server name.`, ``),
        new Parameter(`ripout Network`, `Discovers if *Network* is reachable by consulting the IPX stack's route table and sending out a rip request if necessary.  *Network* is the IPX network segment number.`, ``),
        new Parameter(`resolve{ GUID&#124; name} { GUID&#124; AdapterName}`, `Resolves the name of the GUID to its friendly name, or the friendly name to its GUID.`, ``),
        new Parameter(`board= *N*`, `Specifies the network adapter for which to query or set parameters.`, ``),
        new Parameter(`def`, `Sends packets to the ALL ROUTES broadcast. If a packet is transmitted to a unique Media Access Card (MAC) address that is not in the source routing table, ipxroute sends the packet to the SINGLE ROUTES broadcast by default.`, ``),
        new Parameter(`gbr`, `Sends packets to the ALL ROUTES broadcast. If a packet is transmitted to the broadcast address (FFFFFFFFFFFF), ipxroute sends the packet to the SINGLE ROUTES broadcast by default.`, ``),
        new Parameter(`mbr`, `Sends packets to the ALL ROUTES broadcast. If a packet is transmitted to a multicast address (C000xxxxxxxx), ipxroute sends the packet to the SINGLE ROUTES broadcast by default.`, ``),
        new Parameter(`remove= *xxxxxxxxxxxx*`, `removes the given node address from the source routing table.`, ``),
        new Parameter(`config`, `Displays information about all of the bindings for which IPX is configured.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays and modifies information about the routing tables used by the IPX protocol. Used without parameters,  ipxroute displays the default settings for packets that are sent to unknown, broadcast, and multicast addresses.   `, `ipxroute servers [/type=X]  

ipxroute ripout <Network>  

ipxroute resolve {guid | name} {GUID | <AdapterName>}  

ipxroute board= N [def] [gbr] [mbr] [remove=xxxxxxxxxxxx]  

ipxroute config`, "", () => { }),
    new ConsoleCommand(`irftp`, [
        new Parameter(`D`, `r`, ``),
        new Parameter(`[path]FileName`, `Specifies the location and name of the file or set of files that you want to send over an infrared link. If you specify a set of files, you must specify the full path for each file.`, ``),
        new Parameter(`/h`, `Specifies hidden mode. When hidden mode is used, the files are sent without displaying the Wireless Link dialog box.`, ``),
        new Parameter(`/s`, `Opens the Wireless Link dialog box, so that you can select the file or set of files that you want to send without using the command line to specify the drive, path, and file names.`, ``),
    ], `Sends files over an infrared link.    `, `irftp [<Drive>:\] [[<path>] <FileName>] [/h][/s]`, "", () => { }),
    new ConsoleCommand(`jetpack`, [
        new Parameter(`<database name>`, `Specifies the original database file.`, ``),
        new Parameter(`<temp database name>`, `Specifies the temporary database file.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `compacts a Windows Internet Name Service (WINS) or Dynamic Host Configuration Protocol (DHCP) database. Microsoft recommends that you compact the WINS database whenever it approaches 30 MB. `, `jetpack.EXE <database name> <temp database name>`, "", () => { }),
    new ConsoleCommand(`klist`, [
        new Parameter(`-lh`, `Denotes the high part of the userâ€™s locally unique identifier (LUID), expressed in hexadecimal. If neither â€“lh or â€“li are present, the command defaults to the LUID of the user who is currently signed in.`, ``),
        new Parameter(`-li`, `Denotes the low part of the userâ€™s locally unique identifier (LUID), expressed in hexadecimal. If neither â€“lh or â€“li are present, the command defaults to the LUID of the user who is currently signed in.`, ``),
        new Parameter(`tickets`, `Lists the currently cached ticket-granting-tickets (TGTs), and service tickets of the specified logon session. This is the default option.`, ``),
        new Parameter(`tgt`, `Displays the initial Kerberos TGT.`, ``),
        new Parameter(`purge`, `Allows you to delete all the tickets of the specified logon session.`, ``),
        new Parameter(`sessions`, `Displays a list of logon sessions on this computer.`, ``),
        new Parameter(`kcd_cache`, `Displays the Kerberos constrained delegation cache information.`, ``),
        new Parameter(`get`, `Allows you to request a ticket to the target computer specified by the service principal name (SPN).`, ``),
        new Parameter(`add_bind`, `Allows you to specify a preferred domain controller for Kerberos authentication.`, ``),
        new Parameter(`query_bind`, `Displays a list of cached preferred domain controllers for each domain that Kerberos has contacted.`, ``),
        new Parameter(`purge_bind`, `Removes the cached preferred domain controllers for the domains specified.`, ``),
        new Parameter(`kdcoptions`, `Displays the Key Distribution Center (KDC) options specified in RFC 4120.`, ``),
        new Parameter(`/?`, `Displays Help for this command.`, ``),
    ], `Displays a list of currently cached Kerberos tickets. This information applies to Windows Server 2012. For examples of how this command can be used, see [Examples](#BKMK_Examples).`, `klist [-lh <LogonId.HighPart>] [-li <LogonId.LowPart>] tickets | tgt | purge | sessions | kcd_cache | get | add_bind | query_bind | purge_bind`, "", () => { }),
    new ConsoleCommand(`ksetup`, [
        new Parameter(`[Ksetup:setrealm](ksetup-setrealm.md)`, `Makes this computer a member of a Kerberos realm.`, ``),
        new Parameter(`[Ksetup:mapuser](ksetup-mapuser.md)`, `Maps a Kerberos principal to an account.`, ``),
        new Parameter(`[Ksetup:addkdc](ksetup-addkdc.md)`, `Defines a KDC entry for the given realm.`, ``),
        new Parameter(`[Ksetup:delkdc](ksetup-delkdc.md)`, `Deletes a KDC entry for the realm.`, ``),
        new Parameter(`[Ksetup:addkpasswd](ksetup-addkpasswd.md)`, `Adds a Kpasswd server address for a realm.`, ``),
        new Parameter(`[Ksetup:delkpasswd](ksetup-delkpasswd.md)`, `Deletes a Kpasswd server address for a realm.`, ``),
        new Parameter(`[Ksetup:server](ksetup-server.md)`, `Allows you to specify the name of a Windows computer on which to apply the changes.`, ``),
        new Parameter(`[Ksetup:setcomputerpassword](ksetup-setcomputerpassword.md)`, `Sets the password for the computer's domain account (or host principal).`, ``),
        new Parameter(`[Ksetup:removerealm](ksetup-removerealm.md)`, `Deletes all information for the specified realm from the registry.`, ``),
        new Parameter(`[Ksetup:domain](ksetup-domain.md)`, `Allows you to specify a domain (if <DomainName> has not been set by using /domain).`, ``),
        new Parameter(`[Ksetup:changepassword](ksetup-changepassword.md)`, `Allows you to use the Kpasswd to change the logged on user's password.`, ``),
        new Parameter(`[Ksetup:listrealmflags](ksetup-listrealmflags.md)`, `Lists the available realm flags that ksetup can detect.`, ``),
        new Parameter(`[Ksetup:setrealmflags](ksetup-setrealmflags.md)`, `Sets realm flags for a specific realm.`, ``),
        new Parameter(`[Ksetup:addrealmflags](ksetup-addrealmflags.md)`, `Adds additional realm flags to a realm.`, ``),
        new Parameter(`[Ksetup:delrealmflags](ksetup-delrealmflags.md)`, `Deletes realm flags from a realm.`, ``),
        new Parameter(`[Ksetup:dumpstate](ksetup-dumpstate.md)`, `Analyzes the Kerberos configuration on the given computer. Adds a host to realm mapping to the registry.`, ``),
        new Parameter(`[Ksetup:addhosttorealmmap](ksetup-addhosttorealmmap.md)`, `Adds a registry value to map the host to the Kerberos realm.`, ``),
        new Parameter(`[Ksetup:delhosttorealmmap](ksetup-delhosttorealmmap.md)`, `Deletes the registry value that mapped the host computer to the Kerberos realm.`, ``),
        new Parameter(`[Ksetup:setenctypeattr](ksetup-setenctypeattr.md)`, `Sets one or more encryption types trust attributes for the domain.`, ``),
        new Parameter(`[Ksetup:getenctypeattr](ksetup-getenctypeattr.md)`, `Gets the encryption types trust attribute for the domain.`, ``),
        new Parameter(`[Ksetup:addenctypeattr](ksetup-addenctypeattr.md)`, `Adds encryption types to the encryption types trust attribute for the domain.`, ``),
        new Parameter(`[Ksetup:delenctypeattr](ksetup-delenctypeattr.md)`, `Deletes the encryption types trust attribute for the domain.`, ``),
        new Parameter(`/?`, `Displays Help at the command prompt.`, ``),
    ], `Performs tasks that are related to setting up and maintaining Kerberos protocol and the Key Distribution Center (KDC) to support Kerberos realms, which are not also Windows domains. For examples of how this command can be used, see the Examples section in each of the related subtopics.`, `ksetup 

[/setrealm <DNSDomainName>] 

[/mapuser <Principal> <Account>] 

[/addkdc <RealmName> <KDCName>] 

[/delkdc <RealmName> <KDCName>]

[/addkpasswd <RealmName> <KDCPasswordName>] 

[/delkpasswd <RealmName> <KDCPasswordName>]

[/server <ServerName>] 

[/setcomputerpassword <Password>]

[/removerealm <RealmName>]  

[/domain <DomainName>] 

[/changepassword <OldPassword> <NewPassword>] 

[/listrealmflags] 

[/setrealmflags <RealmName> [sendaddress] [tcpsupported] [delegate] [ncsupported] [rc4]] 

[/addrealmflags <RealmName> [sendaddress] [tcpsupported] [delegate] [ncsupported] [rc4]] 

[/delrealmflags [sendaddress] [tcpsupported] [delegate] [ncsupported] [rc4]] 

[/dumpstate]

[/addhosttorealmmap] <HostName> <RealmName>]  

[/delhosttorealmmap] <HostName> <RealmName>]  

[/setenctypeattr] <DomainName> {DES-CBC-CRC | DES-CBC-MD5 | RC4-HMAC-MD5 | AES128-CTS-HMAC-SHA1-96 | AES256-CTS-HMAC-SHA1-96}

[/getenctypeattr] <DomainName>

[/addenctypeattr] <DomainName> {DES-CBC-CRC | DES-CBC-MD5 | RC4-HMAC-MD5 | AES128-CTS-HMAC-SHA1-96 | AES256-CTS-HMAC-SHA1-96}

[/delenctypeattr] <DomainName>`, "", () => { }),
    new ConsoleCommand(`ktmutil`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
    ], `Starts the Kernel Transaction Manager utility. If used without parameters, ktmutil displays available subcommands.`, `ktmutil list tms 

ktmutil list transactions [{TmGuid}]

ktmutil resolve complete {TmGuid} {RmGuid} {EnGuid}

ktmutil resolve commit {TxGuid}

ktmutil resolve rollback {TxGuid}

ktmutil force commit {??Guid}

ktmutil force rollback {??Guid}

ktmutil forget`, "", () => { }),
    new ConsoleCommand(`ktpass`, [
        new Parameter(`/out <FileName>`, `Specifies the name of the Kerberos version 5 .keytab file to generate. Note: This is the .keytab file that you transfer to a computer that is not running the Windows operating system, and then replace or merge with your existing .keytab file, /Etc/Krb5.keytab.`, ``),
        new Parameter(`/princ <PrincipalName>`, `Specifies the principal name in the form host/computer.contoso.com@CONTOSO.COM. Warning: This parameter is case sensitive. See [remarks](#BKMK_remarks) for more information.`, ``),
        new Parameter(`/mapuser <UserAccount>`, `Maps the name of the Kerberos principal, which is specified by the princ parameter, to the specified domain account.`, ``),
        new Parameter(`/mapop {add&#124;set}`, `Specifies how the mapping attribute is set.<br /><br />-   add adds the value of the specified local user name. This is the default.<br />-   Set sets the value for Data Encryption Standard (DES)-only encryption for the specified local user name.`, ``),
        new Parameter(`{-&#124;+}desonly`, `DES-only encryption is set by default.<br /><br />-   + Sets an account for DES-only encryption.<br />-   - Releases restriction on an account for DES-only encryption. IMPORTANT: Beginning with  Windows 7  and  Windows Server 2008 R2 , Windows does not support DES by default.`, ``),
        new Parameter(`/in <FileName>`, `Specifies the .keytab file to read from a host computer that is not running the Windows operating system.`, ``),
        new Parameter(`/pass {Password&#124;*&#124;{-&#124;+}rndpass}`, `Specifies a password for the principal user name that is specified by the princ parameter. Use "*" to prompt for a password.`, ``),
        new Parameter(`/minpass`, `Sets the minimum length of the random password to 15 characters.`, ``),
        new Parameter(`/maxpass`, `Sets the maximum length of the random password to 256 characters.`, ``),
        new Parameter(`/crypto {DES-CBC-CRC&#124;DES-CBC-MD5&#124;RC4-HMAC-NT&#124;AES256-SHA1&#124;AES128-SHA1&#124;All}`, `Specifies the keys that are generated in the keytab file:<br /><br />-   DES-CBC-CRC is used for compatibility.<br />-   DES-CBC-MD5 adheres more closely to the MIT implementation and is used for compatibility.<br />-   RC4-HMAC-NT employs 128-bit encryption.<br />-   AES256-SHA1 employs AES256-CTS-HMAC-SHA1-96 encryption.<br />-   AES128-SHA1 employs AES128-CTS-HMAC-SHA1-96 encryption.<br />-   All states that all supported cryptographic types can be used. Note: The default settings are based on older MIT versions. Therefore, "/crypto" should always be specified.`, ``),
        new Parameter(`/itercount`, `Specifies the iteration count that is used for AES encryption. The default is that itercount is ignored for non-AES encryption and set at 4,096 for AES encryption.`, ``),
        new Parameter(`/ptype {KRB5_NT_PRINCIPAL&#124;KRB5_NT_SRV_INST&#124;KRB5_NT_SRV_HST}`, `Specifies the principal type.<br /><br />-   KRB5_NT_PRINCIPAL is the general principal type (recommended).<br />-   KRB5_NT_SRV_INST is the user service instance.<br />-   KRB5_NT_SRV_HST is the host service instance.`, ``),
        new Parameter(`/kvno <KeyversionNum>`, `Specifies the key version number. The default value is 1.`, ``),
        new Parameter(`/answer {-&#124;+}`, `Sets the background answer mode:<br /><br />- Answers reset password prompts automatically with NO.<br /><br />+ Answers reset password prompts automatically with YES.`, ``),
        new Parameter(`/target`, `Sets which domain controller to use. The default is for the domain controller to be detected, based on the principal name. If the domain controller name does not resolve, a dialog box will prompt for a valid domain controller.`, ``),
        new Parameter(`/rawsalt`, `forces ktpass to use the rawsalt algorithm when generating the key. This parameter is not needed.`, ``),
        new Parameter(`{-&#124;+}dumpsalt`, `The output of this parameter shows the MIT salt algorithm that is being used to generate the key.`, ``),
        new Parameter(`{-&#124;+}setupn`, `Sets the user principal name (UPN) in addition to the service principal name (SPN). The default is to set both in the .keytab file.`, ``),
        new Parameter(`{-&#124;+}setpass <Password>`, `Sets the user's password when supplied. If rndpass is used, a random password is generated instead.`, ``),
        new Parameter(`/?&#124;/h&#124;/help`, `Displays command-line help for ktpass.`, ``),
    ], `Configures the server principal name for the host or service in active directory Domain Services (AD DS) and generates a .keytab file that contains the shared secret key of the service. The .keytab file is based on the Massachusetts Institute of Technology (MIT) implementation of the Kerberos authentication protocol. The ktpass command-line tool allows non-Windows services that support Kerberos authentication to use the interoperability features provided by the Kerberos Key Distribution Center (KDC) service. This topic applies to the operating system versions designated in the Applies To list at the beginning of the topic.  `, `ktpass  

[/out <FileName>]   

[/princ <PrincipalName>]   

[/mapuser <UserAccount>]   

[/mapop {add|set}] [{-|+}desonly] [/in <FileName>]  

[/pass {Password|*|{-|+}rndpass}]  

[/minpass]  

[/maxpass]  

[/crypto {DES-CBC-CRC|DES-CBC-MD5|RC4-HMAC-NT|AES256-SHA1|AES128-SHA1|All}]  

[/itercount]  

[/ptype {KRB5_NT_PRINCIPAL|KRB5_NT_SRV_INST|KRB5_NT_SRV_HST}]  

[/kvno <KeyversionNum>]  

[/answer {-|+}]  

[/target]  

[/rawsalt] [{-|+}dumpsalt] [{-|+}setupn] [{-|+}setpass <Password>]  [/?|/h|/help]`, "", () => { }),
    new ConsoleCommand(`label`, [
        new Parameter(`/mp`, `Specifies that the volume should be treated as a mount point or volume name.`, ``),
        new Parameter(`<Volume>`, `Specifies a drive letter (followed by a colon), mount point, or volume name. If a volume name is specified, the /mp parameter is unnecessary.`, ``),
        new Parameter(`<Label>`, `Specifies the label for the volume.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Creates, changes, or deletes the volume label (that is, the name) of a disk. If used without parameters, the label command changes the current volume label or deletes the existing label.`, `label [/mp] [<Volume>] [<Label>]`, "", () => { }),
    new ConsoleCommand(`lodctr`, [
        new Parameter(`<filename>`, `Registers the Performance counter name settings and Explain text provided in initialization file FileName.`, ``),
        new Parameter(`/s:<filename>`, `Saves Performance counter registry settings and Explain text to file <filename>.`, ``),
        new Parameter(`/r`, `Restores counter registry settings and Explain text from current registry settings and cached performance files related to the registry.<br /><br />This option is available only in the Windows Server 2003 operating system.`, ``),
        new Parameter(`/r:<filename>`, `Restores Performance counter registry settings and Explain text from file <filename>. Warning: if you use the lodctr /r command, you will overwrite all Performance counter registry settings and Explain text, replacing them with the configuration defined in the file specified.`, ``),
        new Parameter(`/t:<servicename>`, `Indicates that service <servicename> is trusted.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Allows you to register or save performance counter name and registry settings in a file and designate trusted services.`, `lodctr <filename> [/s:<filename>] [/r:<filename>] [/t:<servicename>]`, "", () => { }),
    new ConsoleCommand(`logman`, [
        new Parameter(`[logman create](logman-create.md)`, `create a counter, trace, configuration data collector, or API.`, ``),
        new Parameter(`[logman query](logman-query.md)`, `query data collector properties.`, ``),
        new Parameter(`[logman start &#124; stop](logman-start-stop.md)`, `start or stop data collection.`, ``),
        new Parameter(`[logman delete](logman-delete.md)`, `delete an existing data collector.`, ``),
        new Parameter(`[logman update](logman-update.md)`, `Update the properties of an existing data collector.`, ``),
        new Parameter(`[logman import &#124; export](logman-import-export.md)`, `import a data collector set from an XML file or export a data collector set to an XML file.`, ``),
    ], `logman creates and manages Event Trace Session and Performance logs and supports many functions of Performance Monitor from the command line.`, `logman [create | query | start | stop | delete| update | import | export | /?] [options]`, "", () => { }),
    new ConsoleCommand(`logoff`, [
        new Parameter(`<SessionName>`, `Specifies the name of the session.`, ``),
        new Parameter(`<SessionID>`, `Specifies the numeric ID which identifies the session to the server.`, ``),
        new Parameter(`/server:<ServerName>`, `Specifies the rd Session Host server that contains the session whose user you want to log off. If unspecified, the server on which you are currently active is used.`, ``),
        new Parameter(`/v`, `Displays information about the actions being performed.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Logs off a user from a session on a Remote Desktop Session Host (rd Session Host) server and deletes the session from the server.`, `logoff [<SessionName> | <SessionID>] [/server:<ServerName>] [/v]`, "", () => { }),
    new ConsoleCommand(`lpq`, [
        new Parameter(`-S <ServerName>`, `Specifies (by name or IP address) the computer or printer sharing device that hosts the LPD print queue with a status that you want to display. Required.`, ``),
        new Parameter(`-P <printerName>`, `Specifies (by name) the printer for the print queue with a status that you want to display. Required.`, ``),
        new Parameter(`-l`, `Specifies that you want to display details about the status of the print queue.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays the status of a print queue on a computer running Line printer Daemon (LPD).  `, `lpq -S <ServerName> -P <printerName> [-l]`, "", () => { }),
    new ConsoleCommand(`lpr`, [
        new Parameter(`-S <ServerName>`, `Specifies (by name or IP address) the computer or printer sharing device that hosts the LPD print queue with a status that you want to display. Required.`, ``),
        new Parameter(`-P <printerName>`, `Specifies (by name) the printer for the print queue with a status that you want to display. Required.`, ``),
        new Parameter(`-C <BannerContent>`, `Specifies the content to print on the banner page of the print job. If you do not include this parameter, the name of the computer from which the print job was sent appears on the banner page.`, ``),
        new Parameter(`-J <JobName>`, `Specifies the print job name that will be printed on the banner page. If you do not include this parameter, the name of the file being printed appears on the banner page.`, ``),
        new Parameter(`[-o&#124; "-o l"]`, `Specifies the type of file that you want to print. The parameter -o specifies that you want to print a text file. The parameter "-o l" specifies that you want to print a binary file (for example, a PostScript file).`, ``),
        new Parameter(`-d`, `Specifies that the data file must be sent before the control file. Use this parameter if your printer requires the data file to be sent first. For more information, see your printer documentation.`, ``),
        new Parameter(`-x`, `Specifies that the lpr command must be compatible with the Sun Microsystems operating system (referred to as SunOS) for releases up to and including 4.1.4_u1.`, ``),
        new Parameter(`<FileName>`, `Specifies (by name) the file to be printed. Required.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Sends a file to a computer or printer sharing device running the Line printer Daemon (LPD) service in preparation for printing.  `, `lpr [-S <ServerName>] -P <printerName> [-C <BannerContent>] [-J <JobName>] [-o | "-o l"] [-x] [-d] <filename>`, "", () => { }),
    new ConsoleCommand(`macfile`, [
        new Parameter(`First`, `OwnerSeeFiles`, ``),
        new Parameter(`Second`, `OwnerSeeFolders`, ``),
        new Parameter(`Third`, `OwnerMakechanges`, ``),
        new Parameter(`Fourth`, `GroupSeeFiles`, ``),
        new Parameter(`Fifth`, `GroupSeeFolders`, ``),
        new Parameter(`Sixth`, `GroupMakechanges`, ``),
        new Parameter(`Seventh`, `WorldSeeFiles`, ``),
        new Parameter(`Eighth`, `WorldSeeFolders`, ``),
        new Parameter(`Ninth`, `WorldMakechanges`, ``),
        new Parameter(`Tenth`, `The directory cannot be renamed, moved, or deleted.`, ``),
        new Parameter(`Eleventh`, `The changes apply to the current directory and all subdirectories.`, ``),
    ], `Manages File Server for Macintosh servers, volumes, directories, and files. You can automate administrative tasks by including a series of commands in batch files and starting them manually or at predetermined times. `, `macfile directory[/server:\\<computerName>] /path:<directory> [/owner:<OwnerName>] [/group:<GroupName>] [/permissions:<Permissions>]`, "", () => { }),
    new ConsoleCommand(`makecab`, [
        new Parameter(`<source>`, `File to compress.`, ``),
        new Parameter(`<destination>`, `File name to give compressed file. If omitted, the last character of the source file name is replaced with an underscore (_) and used as the destination.`, ``),
        new Parameter(`/f <directives_file>`, `A file with makecab directives (may be repeated).`, ``),
        new Parameter(`/d var=<value>`, `Defines variable with specified value.`, ``),
        new Parameter(`/l <dir>`, `Location to place destination (default is current directory).`, ``),
        new Parameter(`/v[<n>]`, `Set debugging verbosity level (0=none,...,3=full).`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Package existing files into a cabinet (.cab) file.`, `makecab [/v[n]] [/d var=<value> ...] [/l <dir>] <source> [<destination>]

makecab [/v[<n>]] [/d var=<value> ...] /f <directives_file> [...]`, "", () => { }),
    new ConsoleCommand(`manage-bde`, [
        new Parameter(`[Manage-bde: status](manage-bde-status.md)`, `Provides information about all drives on the computer, whether or not they are BitLocker-protected.`, ``),
        new Parameter(`[Manage-bde: on](manage-bde-on.md)`, `Encrypts the drive and turns on BitLocker.`, ``),
        new Parameter(`[Manage-bde: off](manage-bde-off.md)`, `Decrypts the drive and turns off BitLocker. All key protectors are removed when decryption is complete.`, ``),
        new Parameter(`[Manage-bde: pause](manage-bde-pause.md)`, `Pauses encryption or decryption.`, ``),
        new Parameter(`[Manage-bde: resume](manage-bde-resume.md)`, `Resumes encryption or decryption.`, ``),
        new Parameter(`[Manage-bde: lock](manage-bde-lock.md)`, `Prevents access to BitLocker-protected data.`, ``),
        new Parameter(`[Manage-bde: unlock](manage-bde-unlock.md)`, `Allows access to BitLocker-protected data with a recovery password or a recovery key.`, ``),
        new Parameter(`[Manage-bde: autounlock](manage-bde-autounlock.md)`, `Manages automatic unlocking of data drives.`, ``),
        new Parameter(`[Manage-bde: protectors](manage-bde-protectors.md)`, `Manages protection methods for the encryption key.`, ``),
        new Parameter(`[Manage-bde: tpm](manage-bde-tpm.md)`, `Configures the computer's Trusted Platform Module (TPM). This command is not supported on computers running Windows 8 or win8_server_2. To manage the TPM on these computers, use either the TPM Management MMC snap-in or the TPM Management cmdlets for Windows PowerShell.`, ``),
        new Parameter(`[Manage-bde: setidentifier](manage-bde-setidentifier.md)`, `Sets the drive identifier field on the drive to the value specified in the Provide the unique identifiers for your organization Group Policy setting.`, ``),
        new Parameter(`[Manage-bde: ForceRecovery](manage-bde-forcerecovery.md)`, `Forces a BitLocker-protected drive into recovery mode on restart. This command deletes all TPM-related key protectors from the drive. When the computer restarts, only a recovery password or recovery key can be used to unlock the drive.`, ``),
        new Parameter(`[Manage-bde: changepassword](manage-bde-changepassword.md)`, `Modifies the password for a data drive.`, ``),
        new Parameter(`[Manage-bde: changepin](manage-bde-changepin.md)`, `Modifies the PIN for an operating system drive.`, ``),
        new Parameter(`[Manage-bde: changekey](manage-bde-changekey.md)`, `Modifies the startup key for an operating system drive.`, ``),
        new Parameter(`[Manage-bde: KeyPackage](manage-bde-keypackage.md)`, `Generates a key package for a drive.`, ``),
        new Parameter(`[Manage-bde: upgrade](manage-bde-upgrade.md)`, `Upgrades the BitLocker version.`, ``),
        new Parameter(`[Manage-bde: WipeFreeSpace](manage-bde-wipefreespace.md)`, `Wipes the free space on a drive.`, ``),
        new Parameter(`-? or /?`, `Displays brief Help at the command prompt.`, ``),
        new Parameter(`-help or -h`, `Displays complete Help at the command prompt.`, ``),
    ], `Used to turn on or turn off BitLocker, specify unlock mechanisms, update recovery methods, and unlock BitLocker-protected data drives. This command-line tool can be used in place of the BitLocker Drive Encryption Control Panel item. For examples of how this command can be used, see [Examples](#BKMK_Examples).`, `manage-bde [-status] [â€“on] [â€“off] [â€“pause] [â€“resume] [â€“lock] [â€“unlock] [â€“autounlock] [â€“protectors] [â€“tpm] 

[â€“SetIdentifier] [-ForceRecovery] [â€“changepassword] [â€“changepin] [â€“changekey] [-KeyPackage] [â€“upgrade] [-WipeFreeSpace] [{-?|/?}] [{-help|-h}]`, "", () => { }),
    new ConsoleCommand(`mapadmin`, [
        new Parameter(`-wu &lt;name&gt;`, `Specifies the name of the Windows user for which a new mapping is being created.`, ``),
        new Parameter(`-uu &lt;name&gt;`, `Specifies the name of the UNIX user for which a new mapping is being created.`, ``),
        new Parameter(`-wg &lt;group&gt;`, `Specifies the name of the Windows group for which a new mapping is being created.`, ``),
        new Parameter(`-ug &lt;group&gt;`, `Specifies the name of the UNIX group for which a new mapping is being created.`, ``),
        new Parameter(`-setprimary`, `Specifies that the new mapping is the primary mapping.`, ``),
    ], `You can use Mapadmin to manage User Name Mapping for Microsoft Services for Network File System.`, `mapadmin [<computer>] [-u <user> [-p <password>]]

mapadmin [<computer>] [-u <user> [-p <password>]] {start | stop}

mapadmin [<computer>] [-u <user> [-p <password>]] config <option[...]>

mapadmin [<computer>] [-u <user> [-p <password>]] add -wu <WindowsUser> -uu <UNIXUser> [-setprimary]

mapadmin [<computer>] [-u <user> [-p <password>]] add -wg <WindowsGroup> -ug <UNIXGroup> [-setprimary]

mapadmin [<computer>] [-u <user> [-p <password>]] setprimary -wu <WindowsUser> [-uu <UNIXUser>]

mapadmin [<computer>] [-u <user> [-p <password>]] setprimary -wg <WindowsGroup> [-ug <UNIXGroup>]

mapadmin [<computer>] [-u <user> [-p <password>]] delete <option[...]>

mapadmin [<computer>] [-u <user> [-p <password>]] list <option[...]>

mapadmin [<computer>] [-u <user> [-p <password>]] backup <filename> 

mapadmin [<computer>] [-u <user> [-p <password>]] restore <filename>

mapadmin [<computer>] [-u <user> [-p <password>]] adddomainmap -d <WindowsDomain> {-y <<NISdomain>> | -f <path>}

mapadmin [<computer>] [-u <user> [-p <password>]] removedomainmap -d <WindowsDomain> -y <<NISdomain>>

mapadmin [<computer>] [-u <user> [-p <password>]] removedomainmap -all

mapadmin [<computer>] [-u <user> [-p <password>]] listdomainmaps`, "", () => { }),
    new ConsoleCommand(`Md`, [
        new Parameter(`<Drive>:`, `Specifies the drive on which you want to create the new directory.`, ``),
        new Parameter(`<Path>`, `Required. Specifies the name and location of the new directory. The maximum length of any single path is determined by the file system.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Creates a directory or subdirectory.`, `md [<Drive>:]<Path>

mkdir [<Drive>:]<Path>`, "", () => { }),
    new ConsoleCommand(`mkdir`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `This command is the same as the md command. See [Md](md.md) for syntax and parameters.`, ``, "", () => { }),
    new ConsoleCommand(`mklink`, [
        new Parameter(`/d`, `Creates a directory symbolic link. By default, mklink creates a file symbolic link.`, ``),
        new Parameter(`/h`, `Creates a hard link instead of a symbolic link.`, ``),
        new Parameter(`/j`, `Creates a Directory Junction.`, ``),
        new Parameter(`<Link>`, `Specifies the name of the symbolic link that is being created.`, ``),
        new Parameter(`<Target>`, `Specifies the path (relative or absolute) that the new symbolic link refers to.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Creates a symbolic link.`, `mklink [[/d] | [/h] | [/j]] <Link> <Target>`, "", () => { }),
    new ConsoleCommand(`mmc`, [
        new Parameter(`<path>\<filename>.msc`, `starts mmc and opens a saved console. You need to specify the complete path and file name for the saved console file. If you do not specify a console file, mmc opens a new console.`, ``),
        new Parameter(`/a`, `Opens a saved console in author mode.  Used to make changes to saved consoles.`, ``),
        new Parameter(`/64`, `Opens the 64-bit version of mmc (mmc64). Use this option only if you are running a Microsoft 64-bit operating system and want to use a 64-bit snap-in.`, ``),
        new Parameter(`/32`, `Opens the 32-bit version of mmc (mmc32). When running a Microsoft 64-bit operating system, you can run 32-bit snap-ins by opening mmc with this command-line option when you have 32-bit only snap-ins.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Using mmc command-line options, you can open a specific mmc console, open mmc in author mode, or specify that the 32-bit or 64-bit version of mmc is opened.`, `mmc <path>\<filename>.msc [/a] [/64] [/32]`, "", () => { }),
    new ConsoleCommand(`mode`, [
        new Parameter(`Com<M>[:]`, `Specifies the number of the async Prncnfg.vbshronous communications port.`, ``),
        new Parameter(`baud=<B>`, `Specifies the transmission rate in bits per second. The following table lists valid abbreviations for *B* and their related rates.</br>-   11 = 110 baud</br>-   15 = 150 baud</br>-   30 = 300 baud</br>-   60 = 600 baud</br>-   12 = 1200 baud</br>-   24 = 2400 baud</br>-   48 = 4800 baud</br>-   96 = 9600 baud</br>-   19 = 19,200 baud`, ``),
        new Parameter(`parity=<P>`, `Specifies how the system uses the parity bit to check for transmission errors. The following table lists valid values for *P*. The default value is e. Not all computers support the values m and s.</br>-   n = none</br>-   e = even</br>-   o = odd</br>-   m = mark</br>-   s = space`, ``),
        new Parameter(`data=<D>`, `Specifies the number of data bits in a character. Valid values for d are in the range 5 through 8. The default value is 7. Not all computers support the values 5 and 6.`, ``),
        new Parameter(`stop=<S>`, `Specifies the number of stop bits that define the end of a character: 1, 1.5, or 2. If the baud rate is 110, the default value is 2. Otherwise, the default value is 1. Not all computers support the value 1.5.`, ``),
        new Parameter(`to={on `, ` off}`, ``),
        new Parameter(`xon={on `, ` off}`, ``),
        new Parameter(`odsr={on `, ` off}`, ``),
        new Parameter(`octs={on `, ` off}`, ``),
        new Parameter(`dtr={on `, ` off `, ``),
        new Parameter(`rts={on `, ` off `, ``),
        new Parameter(`idsr={on `, ` off}`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays system status, changes system settings, or reconfigures ports or devices. If used without parameters, mode displays all the controllable attributes of the console and the available COM devices.`, `mode com<M>[:] [baud=<B>] [parity=<P>] [data=<D>] [stop=<S>] [to={on|off}] [xon={on|off}] [odsr={on|off}] [octs={on|off}] [dtr={on|off|hs}] [rts={on|off|hs|tg}] [idsr={on|off}]`, "", () => { }),
    new ConsoleCommand(`more`, [
        new Parameter(`<Command>`, `Specifies a command for which you want to display the output.`, ``),
        new Parameter(`/c`, `Clears the screen before displaying a page.`, ``),
        new Parameter(`/p`, `Expands form-feed characters.`, ``),
        new Parameter(`/s`, `Displays multiple blank lines as a single blank line.`, ``),
        new Parameter(`/t<N>`, `Displays tabs as the number of spaces specified by *N*.`, ``),
        new Parameter(`+<N>`, `Displays the first file beginning at the line specified by *N*.`, ``),
        new Parameter(`[<Drive>:] [<Path>]<FileName>`, `Specifies the location and name of a file to display.`, ``),
        new Parameter(`<Files>`, `Specifies a list of files to display. Separate file names with a space.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays one screen of output at a time.`, `<Command> | more [/c] [/p] [/s] [/t<N>] [+<N>]

more [[/c] [/p] [/s] [/t<N>] [+<N>]] < [<Drive>:][<Path>]<FileName>

more [/c] [/p] [/s] [/t<N>] [+<N>] [<Files>]`, "", () => { }),
    new ConsoleCommand(`mount`, [
        new Parameter(`-o rsize=<buffersize>`, `Sets the size in kilobytes of the read buffer. Acceptable values are 1, 2, 4, 8, 16, and 32; the default is 32 KB.`, ``),
        new Parameter(`-o wsize=<buffersize>`, `Sets the size in kilobytes of the write buffer. Acceptable values are 1, 2, 4, 8, 16, and 32; the default is 32 KB.`, ``),
        new Parameter(`-o timeout=<seconds>`, `Sets the time-out value in seconds for a remote procedure call (RPC). Acceptable values are 0.8, 0.9, and any integer in the range 1-60; the default is 0.8.`, ``),
        new Parameter(`-o retry=<number>`, `Sets the number of retries for a soft mount. Acceptable values are integers in the range 1-10; the default is 1.`, ``),
        new Parameter(`-o mtype={soft `, ` hard}`, ``),
        new Parameter(`-o anon`, `Mounts as an anonymous user.`, ``),
        new Parameter(`-o nolock`, `Disables locking (default is enabled).`, ``),
        new Parameter(`-o casesensitive`, `Forces file lookups on the server to be case sensitive.`, ``),
        new Parameter(`-o fileaccess=<mode>`, `Specifies the default permission mode of new files created on the NFS share. Specify *mode* as a three-digit number in the form *ogw*, where *o*, *g*, and *w* are each a digit representing the access granted the file's owner, group, and the world, respectively. The digits must be in the range 0-7 with the following meaning:</br>-   0: No access</br>-   1: x (execute access)</br>-   2: w (write access)</br>-   3: wx</br>-   4: r (read access)</br>-   5: rx</br>-   6: rw</br>-   7: rwx`, ``),
        new Parameter(`-o lang={euc-jp`, `euc-tw`, ``),
        new Parameter(`-u:<UserName>`, `Specifies the user name to use for mounting the share. If *username* is not preceded by a backslash (), it is treated as a UNIX user name.`, ``),
        new Parameter(`-p:<Password>`, `The password to use for mounting the share. If you use an asterisk (*), you will be prompted for the password.`, ``),
    ], `You can use mount to mount Network File System (NFS) network shares.`, `mount [-o <Option>[...]] [-u:<UserName>] [-p:{<Password> | *}] {\\<ComputerName>\<ShareName> | <ComputerName>:/<ShareName>} {<DeviceName> | *}`, "", () => { }),
    new ConsoleCommand(`mountvol`, [
        new Parameter(`[<Drive>:]<Path>`, `Specifies the existing NTFS directory where the mount point will reside.`, ``),
        new Parameter(`<VolumeName>`, `Specifies the volume name that is the target of the mount point. The volume name uses the following syntax, where *GUID* is a globally unique identifier:</br>"\\\?Volume{GUID}"</br>The brackets { } are required.`, ``),
        new Parameter(`/d`, `Removes the volume mount point from the specified folder.`, ``),
        new Parameter(`/l`, `Lists the mounted volume name for the specified folder.`, ``),
        new Parameter(`/p`, `Removes the volume mount point from the specified directory, dismounts the basic volume, and takes the basic volume offline, making it unmountable. If other processes are using the volume, mountvol closes any open handles before dismounting the volume.`, ``),
        new Parameter(`/r`, `Removes volume mount point directories and registry settings for volumes that are no longer in the system, preventing them from being automatically mounted and given their former volume mount point(s) when added back to the system.`, ``),
        new Parameter(`/n`, `Disables automatic mounting of new basic volumes. New volumes are not mounted automatically when added to the system.`, ``),
        new Parameter(`/e`, `Re-enables automatic mounting of new basic volumes.`, ``),
        new Parameter(`/s`, `Mounts the EFI system partition on the specified drive. Available on Itanium-based computers only.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Creates, deletes, or lists a volume mount point.`, `mountvol [<Drive>:]<Path VolumeName>

mountvol [<Drive>:]<Path> /d

mountvol [<Drive>:]<Path> /l

mountvol [<Drive>:]<Path> /p

mountvol /r

mountvol [/n | /e]

mountvol <Drive>: /s`, "", () => { }),
    new ConsoleCommand(`move`, [
        new Parameter(`/y`, `Suppresses prompting to confirm that you want to overwrite an existing destination file.`, ``),
        new Parameter(`/-y`, `Causes prompting to confirm that you want to overwrite an existing destination file.`, ``),
        new Parameter(`<Source>`, `Specifies the path and name of the file or files to move. If you want to move or rename a directory, *Source* should be the current directory path and name.`, ``),
        new Parameter(`<Target>`, `Specifies the path and name to move files to. If you want to move or rename a directory, *Target* should be the desired directory path and name.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Moves one or more files from one directory to another directory.`, `move [{/y | /-y}] [<Source>] [<Target>]`, "", () => { }),
    new ConsoleCommand(`mqbkup`, [
        new Parameter(`/b`, `Specifies backup operation`, ``),
        new Parameter(`/r`, `Specifies restore operation`, ``),
        new Parameter(`<folder path_to_storage_device>`, `Specifies the path where the MSMQ message files and registry settings are stored`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Backs up MSMQ message files and registry settings to a storage device and restores previously-stored messages and settings.   `, `mqbkup {/b | /r} <folder path_to_storage_device>`, "", () => { }),
    new ConsoleCommand(`mqsvc`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
    ], `Message Queuing technology enables applications running at different times to communicate across heterogeneous networks and systems that may be temporarily offline. Message Queuing provides guaranteed message delivery, efficient routing, security, and priority-based messaging. It can be used to implement solutions for both asynchronous and synchronous messaging scenarios. For more information about this command, see [Message Queuing (MSMQ)](https://go.microsoft.com/fwlink/?LinkId=248723) on MSDN.`, `Mqsvc.exe`, "", () => { }),
    new ConsoleCommand(`mqtgsvc`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Monitors a queue for incoming messages and performs an action, in the form of an executable file or COM component, when the rules of a trigger are evaluated as true. For examples of how the Message Queuing Triggers service can be used, see [Message Queuing Triggers](https://go.microsoft.com/fwlink/?LinkId=248725)on MSDN.`, `Mqtgsvc.exe`, "", () => { }),
    new ConsoleCommand(`msdt`, [
        new Parameter(`/id <package name>`, `Specifies which diagnostic package to run. For a list of available packages, see the Troubleshooting Pack ID in the â€œAvailable troubleshooting packsâ€? section later in this topic.`, ``),
        new Parameter(`/path <directory `, ` .diagpkg file `, ``),
        new Parameter(`/dci <passkey>`, `Prepopulates the passkey field in msdt. This parameter is only used when a support provider has supplied a passkey.`, ``),
        new Parameter(`/dt <directory>`, `Displays the troubleshooting history in the specified directory. Diagnostic results are stored in the userâ€™s %LOCALAPPDATA%Diagnostics or %LOCALAPPDATA%ElevatedDiagnostics directories.`, ``),
        new Parameter(`/af <answer file>`, `Specifies an answer file in XML format that contains responses to one or more diagnostic interactions.`, ``),
    ], `Invokes a troubleshooting pack at the command line or as part of an automated script, and enables additional options without user input.`, `msdt </id <name> | /path <name> | /cab < name>> <</parameter> [options] â€¦ <parameter> [options]>>`, "", () => { }),
    new ConsoleCommand(`msg`, [
        new Parameter(`<UserName>`, `Specifies the name of the user that you want to receive the message.`, ``),
        new Parameter(`<SessionName>`, `Specifies the name of the session that you want to receive the message.`, ``),
        new Parameter(`<SessionID>`, `Specifies the numeric ID of the session whose user you want to receive a message.`, ``),
        new Parameter(`@<FileName>`, `Identifies a file containing a list of user names, session names, and session IDs that you want to receive the message.`, ``),
        new Parameter(`*`, `Sends the message to all user names on the system.`, ``),
        new Parameter(`/server:<ServerName>`, `Specifies the rd Session Host server whose session or user you want to receive the message. If unspecified, /server uses the server to which you are currently logged on.`, ``),
        new Parameter(`/time:<Seconds>`, `Specifies the amount of time that the message you sent is displayed on the user's screen. After the time limit is reached, the message disappears. If no time limit is set, the message remains on the user's screen until the user sees the message and clicks OK.`, ``),
        new Parameter(`/v`, `Displays information about the actions being performed.`, ``),
        new Parameter(`/w`, `Waits for an acknowledgment from the user that the message has been received. Use this parameter with /time:<*Seconds*> to avoid a possible long delay if the user does not immediately respond. Using this parameter with /v is also helpful.`, ``),
        new Parameter(`<Message>`, `Specifies the text of the message that you want to send. If no message is specified, you will be prompted to enter a message. To send a message that is contained in a file, type the less than (<) symbol followed by the file name.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Sends a message to a user on a Remote Desktop Session Host (rd Session Host) server.`, `msg {<UserName> | <SessionName> | <SessionID>| @<FileName> | *} [/server:<ServerName>] [/time:<Seconds>] [/v] [/w] [<Message>]`, "", () => { }),
    new ConsoleCommand(`msiexec`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
    ], `## Msiexec`, ``, "", () => { }),
    new ConsoleCommand(`msinfo32`, [
        new Parameter(`<path>`, `Specifies the file to be opened in the format *C:Folder1File1.XXX*, where *C* is the drive letter, *Folder1* is the folder, *File1* is the file name, and *XXX* is the file name extension.<br /><br />This file can be an .nfo, .xml, .txt, or .cab file.`, ``),
        new Parameter(`<computerName>`, `Specifies the name of the target or local computer. This can be a UNC name, an IP address, or a full computer name.`, ``),
        new Parameter(`<CategoryID>`, `Specifies the ID of the category item. You can obtain the category ID by using /showcategories.`, ``),
        new Parameter(`/pch`, `Displays the System History view in the System Information tool.`, ``),
        new Parameter(`/nfo`, `Saves the exported file as an .nfo file. If the file name that is specified in *path* does not end in an .nfo extension, the .nfo extension is automatically appended to the file name.`, ``),
        new Parameter(`/report`, `Saves the file in *path* as a text file. The file name is saved exactly as it appears in *path*. The .txt extension is not appended to the file unless it is specified in path.`, ``),
        new Parameter(`/computer`, `starts the System Information tool for the specified remote computer. You must have the appropriate permissions to access the remote computer.`, ``),
        new Parameter(`/showcategories`, `starts the System Information tool with all available category IDs displayed, rather than displaying the friendly or localized names. For example, the Software Environment category is displayed as the SWEnv category.`, ``),
        new Parameter(`/category`, `starts System Information with the specified category selected. Use /showcategories to display a list of available category IDs.`, ``),
        new Parameter(`/categories`, `starts System Information with only the specified category or categories displayed. It also limits the output to the selected category or categories. Use /showcategories to display a list of available category IDs.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Opens the System Information tool to display a comprehensive view of the hardware, system components, and software environment on the local computer. `, `msinfo32 [/pch] [/nfo <path>] [/report <path>] [/computer <computerName>] [/showcategories] [/category <CategoryID>] [/categories {+<CategoryID>(+<CategoryID>)|+all(-<CategoryID>)}]`, "", () => { }),
    new ConsoleCommand(`mstsc`, [
        new Parameter(`<Connection File>`, `Specifies the name of an .rdp file for the connection.`, ``),
        new Parameter(`/v:<Server[:<Port>]`, `Specifies the remote computer and, optionally, the port number to which you want to connect.`, ``),
        new Parameter(`/admin`, `Connects you to a session for administering the server.`, ``),
        new Parameter(`/f`, `starts Remote Desktop Connection in full-screen mode.`, ``),
        new Parameter(`/w:<Width>`, `Specifies the width of the Remote Desktop window.`, ``),
        new Parameter(`/h:<Height>`, `Specifies the height of the Remote Desktop window.`, ``),
        new Parameter(`/public`, `Runs Remote Desktop in public mode. In public mode, passwords and bitmaps are not cached.`, ``),
        new Parameter(`/span`, `Matches the Remote Desktop width and height with the local virtual desktop, spanning across multiple monitors if necessary.`, ``),
        new Parameter(`/edit <Connection File>`, `Opens the specified .rdp file for editing.`, ``),
        new Parameter(`/migrate`, `Migrates legacy connection files that were created with Client Connection Manager to new .rdp connection files.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `creates connections to Remote Desktop Session Host (rd Session Host) servers or other remote computers, edits an existing Remote Desktop Connection (.rdp) configuration file, and migrates legacy connection files that were created with Client Connection Manager to new .rdp connection files.`, `mstsc.exe [<Connection File>] [/v:<Server>[:<Port>]] [/admin] [/f] [/w:<Width> /h:<Height>] [/public] [/span]

mstsc.exe /edit <Connection File>

mstsc.exe /migrate`, "", () => { }),
    new ConsoleCommand(`nbtstat`, [
        new Parameter(`/a <remoteName>`, `Displays the NetBIOS name table of a remote computer, where *remoteName* is the NetBIOS computer name of the remote computer. The NetBIOS name table is the list of NetBIOS names that corresponds to NetBIOS applications running on that computer.`, ``),
        new Parameter(`/A <IPaddress>`, `Displays the NetBIOS name table of a remote computer, specified by the IP address (in dotted decimal notation) of the remote computer.`, ``),
        new Parameter(`/c`, `Displays the contents of the NetBIOS name cache, the table of NetBIOS names and their resolved IP addresses.`, ``),
        new Parameter(`/n`, `Displays the NetBIOS name table of the local computer. The status of registered indicates that the name is registered either by broadcast or with a WINS server.`, ``),
        new Parameter(`/r`, `Displays NetBIOS name resolution statistics. On a computer running Windows XP or Windows Server 2003 that is configured to use WINS, this parameter returns the number of names that have been resolved and registered using broadcast and WINS.`, ``),
        new Parameter(`/R`, `Purges the contents of the NetBIOS name cache and then reloads the #PRE-tagged entries from the Lmhosts file.`, ``),
        new Parameter(`/RR`, `Releases and then refreshes NetBIOS names for the local computer that is registered with WINS servers.`, ``),
        new Parameter(`/s`, `Displays NetBIOS client and server sessions, attempting to convert the destination IP address to a name.`, ``),
        new Parameter(`/S`, `Displays NetBIOS client and server sessions, listing the remote computers by destination IP address only.`, ``),
        new Parameter(`<Interval>`, `Redisplays selected statistics, pausing the number of seconds specified in *Interval* between each display. Press CTRL+C to stop redisplaying statistics. If this parameter is omitted, nbtstat prints the current configuration information only once.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays NetBIOS over TCP/IP (NetBT) protocol statistics, NetBIOS name tables for both the local computer and remote computers, and the NetBIOS name cache. nbtstat allows a refresh of the NetBIOS name cache and the names registered with Windows Internet Name Service (WINS). Used without parameters, nbtstat displays help. `, `nbtstat [/a <remoteName>] [/A <IPaddress>] [/c] [/n] [/r] [/R] [/RR] [/s] [/S] [<Interval>]`, "", () => { }),
    new ConsoleCommand(`netcfg`, [
        new Parameter(`/v`, `Run in verbose (detailed) mode`, ``),
        new Parameter(`/e`, `Use servicing environment variables during install and uninstall`, ``),
        new Parameter(`/winpe`, `Installs TCP/IP, NetBIOS and Microsoft Client for Windows preinstallation envrionment`, ``),
        new Parameter(`/l`, `Provides the location of INF`, ``),
        new Parameter(`/c`, `Provides the class of the component to be installed; protocol, Service, or client`, ``),
        new Parameter(`/i`, `Provides the component ID`, ``),
        new Parameter(`/s`, `Provides the type of components to show<br /><br />ta = adapters, n = net components`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Installs the Windows Preinstallation Environment (WinPE), a lightweight version of Windows used to deploy workstations.   `, `netcfg [/v] [/e] [/winpe] [/l ] /c /i`, "", () => { }),
    new ConsoleCommand(`netsh`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Netsh is a command-line scripting utility that allows you to, either locally or remotely, display or modify the network configuration of a currently running computer.`, ``, "", () => { }),
    new ConsoleCommand(`netstat`, [
        new Parameter(`-a`, `Displays all active TCP connections and the TCP and UDP ports on which the computer is listening.`, ``),
        new Parameter(`-e`, `Displays Ethernet statistics, such as the number of bytes and packets sent and received. This parameter can be combined with -s.`, ``),
        new Parameter(`-n`, `Displays active TCP connections, however, addresses and port numbers are expressed numerically and no attempt is made to determine names.`, ``),
        new Parameter(`-o`, `Displays active TCP connections and includes the process ID (PID) for each connection. You can find the application based on the PID on the Processes tab in Windows Task Manager. This parameter can be combined with -a, -n, and -p.`, ``),
        new Parameter(`-p <Protocol>`, `Shows connections for the protocol specified by *Protocol*. In this case, the *Protocol* can be tcp, udp, tcpv6, or udpv6. If this parameter is used with -s to display statistics by protocol, *Protocol* can be tcp, udp, icmp, ip, tcpv6, udpv6, icmpv6, or ipv6.`, ``),
        new Parameter(`-s`, `Displays statistics by protocol. By default, statistics are shown for the TCP, UDP, ICMP, and IP protocols. If the IPv6 protocol is installed, statistics are shown for the TCP over IPv6, UDP over IPv6, ICMPv6, and IPv6 protocols. The -p parameter can be used to specify a set of protocols.`, ``),
        new Parameter(`-r`, `Displays the contents of the IP routing table. This is equivalent to the route print command.`, ``),
        new Parameter(`<Interval>`, `Redisplays the selected information every *Interval* seconds. Press CTRL+C to stop the redisplay. If this parameter is omitted, netstat prints the selected information only once.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays active TCP connections, ports on which the computer is listening, Ethernet statistics, the IP routing table, IPv4 statistics (for the IP, ICMP, TCP, and UDP protocols), and IPv6 statistics (for the IPv6, ICMPv6, TCP over IPv6, and UDP over IPv6 protocols). Used without parameters, netstat displays active TCP connections. `, `netstat [-a] [-e] [-n] [-o] [-p <Protocol>] [-r] [-s] [<Interval>]`, "", () => { }),
    new ConsoleCommand(`nfsadmin`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `You can use nfsadmin to manage Server for NFS and Client for NFS.  `, ``, "", () => { }),
    new ConsoleCommand(`nfsshare`, [
        new Parameter(`-o anon={yes `, ` no}`, ``),
        new Parameter(`-o rw[=<Host>[:<Host>]...]`, `Provides read-write access to the shared directory by the hosts or client groups specified by *Host*. Separate host and group names with a colon (:). If *Host* is not specified, all hosts and client groups (except those specified with the ro option) have read-write access. If neither the ro nor the rw option is set, all clients have read-write access to the shared directory.`, ``),
        new Parameter(`-o ro[=<Host>[:<Host>]...]`, `Provides read-only access to the shared directory by the hosts or client groups specified by *Host*. Separate host and group names with a colon (:). If *Host* is not specified, all clients (except those specified with the rw option) have read-only access. If the ro option is set for one or more clients, but the rw option is not set, only the clients specified with the ro option have access to the shared directory.`, ``),
        new Parameter(`-o encoding={big5`, `euc-jp`, ``),
        new Parameter(`-o anongid=<gid>`, `Specifies that anonymous (unmapped) users will access the share directory using *gid* as their group identifier (GID). The default is -2. The anonymous GID will be used when reporting the owner of a file owned by an unmapped user, even if anonymous access is disabled.`, ``),
        new Parameter(`-o  anonuid=<uid>`, `Specifies that anonymous (unmapped) users will access the share directory using *uid* as their user identifier (UID). The default is -2. The anonymous UID will be used when reporting the owner of a file owned by an unmapped user, even if anonymous access is disabled.`, ``),
        new Parameter(`-o root[=<Host>[:<Host>]...]`, `Provides root access to the shared directory by the hosts or client groups specified by *Host*. Separate host and group names with a colon (:). If *Host* is not specified, all clients have root access. If the root option is not set, no clients have root access to the shared directory.`, ``),
        new Parameter(`/delete`, `If *ShareName* or *Drive*:*Path* is specified, deletes the specified share. If * is specified, deletes all NFS shares.`, ``),
    ], `You can use nfsshare to control Network File System (NFS) shares.`, `nfsshare <ShareName>=<Drive:Path> [-o <Option=value>...]

nfsshare {<ShareName> | <Drive>:<Path> | * } /delete`, "", () => { }),
    new ConsoleCommand(`nfsstat`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `You can use nfsstat to display or reset counts of calls made to Server for NFS.`, `nfsstat [-z]`, "", () => { }),
    new ConsoleCommand(`nlbmgr`, [
        new Parameter(`/help`, `Displays help at the command prompt.`, ``),
        new Parameter(`/noping`, `Prevents Network Load Balancing Manager from pinging the hosts prior to trying to contact them through Windows Management Instrumentation (WMI). Use this option if you have disabled Internet Control Message Protocol (ICMP) on all available network adapters. If Network Load Balancing Manager attempts to contact a host that is not available, you will experience a delay when using this option.`, ``),
        new Parameter(`/hostlist <filename>`, `Loads the hosts specified in filename into Network Load Balancing Manager.`, ``),
        new Parameter(`/autorefresh <interval>`, `Causes Network Load Balancing Manager to refresh its host and cluster information every <interval> seconds. If no interval is specified, the information is refreshed every 60 seconds.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Using Network Load Balancing Manager, you can configure and manage your Network Load Balancing clusters and all cluster hosts from a single computer, and you can also replicate the cluster configuration to other hosts. You can start Network Load Balancing Manager from the command-line using the command nlbmgr.exe, which is installed in the systemroot\System32 folder.`, `nlbmgr [/help] [/noping] [/hostlist <filename>] [/autorefresh <interval>]`, "", () => { }),
    new ConsoleCommand(`nslookup`, [
        new Parameter(`[nslookup exit Command](nslookup-exit-command.md)`, `exits nslookup.`, ``),
        new Parameter(`[nslookup finger Command](nslookup-finger-command.md)`, `Connects with the finger server on the current computer.`, ``),
        new Parameter(`[nslookup help](nslookup-help.md)`, `Displays a short summary of nslookup subcommands.`, ``),
        new Parameter(`[nslookup ls](nslookup-ls.md)`, `lists information for a DNS domain.`, ``),
        new Parameter(`[nslookup lserver](nslookup-lserver.md)`, `changes the default server to the specified DNS domain.`, ``),
        new Parameter(`[nslookup root](nslookup-root.md)`, `changes the default server to the server for the root of the DNS domain name space.`, ``),
        new Parameter(`[nslookup server](nslookup-server.md)`, `changes the default server to the specified DNS domain.`, ``),
        new Parameter(`[nslookup set](nslookup-set.md)`, `changes configuration settings that affect how lookups function.`, ``),
        new Parameter(`[nslookup set all](nslookup-set-all.md)`, `prints the current values of the configuration settings.`, ``),
        new Parameter(`[nslookup set class](nslookup-set-class.md)`, `changes the query class. The class specifies the protocol group of the information.`, ``),
        new Parameter(`[nslookup set d2](nslookup-set-d2.md)`, `Turns exhaustive Debugging mode on or off. All fields of every packet are printed.`, ``),
        new Parameter(`[nslookup set debug](nslookup-set-debug.md)`, `Turns Debugging mode on or off.`, ``),
        new Parameter(`nslookup /set defname`, `appends the default DNS domain name to a single component lookup request. A single component is a component that contains no periods.`, ``),
        new Parameter(`[nslookup set domain](nslookup-set-domain.md)`, `changes the default DNS domain name to the name specified.`, ``),
        new Parameter(`nslookup /set ignore`, `Ignores packet truncation errors.`, ``),
        new Parameter(`[nslookup set port](nslookup-set-port.md)`, `changes the default TCP/UDP DNS name server port to the value specified.`, ``),
        new Parameter(`[nslookup set querytype](nslookup-set-querytype.md)`, `changes the resource record type for the query.`, ``),
        new Parameter(`[nslookup set recurse](nslookup-set-recurse.md)`, `Tells the DNS name server to query other servers if it does not have the information.`, ``),
        new Parameter(`[nslookup set retry](nslookup-set-retry.md)`, `Sets the number of retries.`, ``),
        new Parameter(`[nslookup set root](nslookup-set-root.md)`, `changes the name of the root server used for queries.`, ``),
        new Parameter(`[nslookup set search](nslookup-set-search.md)`, `appends the DNS domain names in the DNS domain search list to the request until an answer is received. This applies when the set and the lookup request contain at least one period, but do not end with a trailing period.`, ``),
        new Parameter(`[nslookup set srchlist](nslookup-set-srchlist.md)`, `changes the default DNS domain name and search list.`, ``),
        new Parameter(`[nslookup set timeout](nslookup-set-timeout.md)`, `changes the initial number of seconds to wait for a reply to a request.`, ``),
        new Parameter(`[nslookup set type](nslookup-set-type.md)`, `changes the resource record type for the query.`, ``),
        new Parameter(`[nslookup set vc](nslookup-set-vc.md)`, `Specifies to use or not use a virtual circuit when sending requests to the server.`, ``),
        new Parameter(`[nslookup view](nslookup-view.md)`, `sorts and lists the output of the previous ls subcommand or commands.`, ``),
    ], `Displays information that you can use to diagnose Domain Name System (DNS) infrastructure. Before using this tool, you should be familiar with how DNS works. The nslookup command-line tool is available only if you have installed the TCP/IP protocol.`, `nslookup [<-SubCommand ...>] [{<computerTofind> | -<Server>}]

nslookup /exit

nslookup /finger [<UserName>] [{[>] <FileName>|[>>] <FileName>}]

nslookup /{help | ?}

nslookup /ls [<Option>] <DNSDomain> [{[>] <FileName>|[>>] <FileName>}]

nslookup /lserver <DNSDomain> 

nslookup /root 

nslookup /server <DNSDomain>

nslookup /set <KeyWord>[=<Value>]

nslookup /set all 

nslookup /set class=<Class>

nslookup /set [no]d2

nslookup /set [no]debug

nslookup /set [no]defname

nslookup /set domain=<DomainName>

nslookup /set [no]ignore

nslookup /set port=<Port>

nslookup /set querytype=<ResourceRecordtype>

nslookup /set [no]recurse

nslookup /set retry=<Number>

nslookup /set root=<RootServer>

nslookup /set [no]search

nslookup /set srchlist=<DomainName>[/...]

nslookup /set timeout=<Number>

nslookup /set type=<ResourceRecordtype>

nslookup /set [no]vc

nslookup /view <FileName>`, "", () => { }),
    new ConsoleCommand(`ntbackup`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `The ntbackup command is not available in Windows Vista or Windows Server 2008. Instead, you should use the wbadmin command and subcommands to back up and restore your computer and files from a command prompt.`, ``, "", () => { }),
    new ConsoleCommand(`ntcmdprompt`, [
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Runs the command interpreter Cmd.exe, rather than Command.com, after running a Terminate and Stay Resident (TSR) or after starting the command prompt from within an MS-DOS application.`, `ntcmdprompt`, "", () => { }),
    new ConsoleCommand(`ntfrsutl`, [
        new Parameter(`idtable`, `ID table`, ``),
        new Parameter(`configtable`, `FRS configuration table`, ``),
        new Parameter(`inlog`, `Inbound log`, ``),
        new Parameter(`outlog`, `Outbound log`, ``),
        new Parameter(`<computer>`, `Specifies the computer.`, ``),
        new Parameter(`memory`, `Memory usage`, ``),
        new Parameter(`t`, `h`, ``),
        new Parameter(`s`, `t`, ``),
        new Parameter(`ds`, `lists the NTFRS service's view of the DS.`, ``),
        new Parameter(`sets`, `Specifies the active replica sets`, ``),
        new Parameter(`version`, `Specifies the API and NTFRS service versions.`, ``),
        new Parameter(`poll`, `Specifies the current polling intervals.<br /><br />Parameters:<br /><br /><ul><li>/quickly[=[ <N>]]  (Polls quickly)<br /><br /><ul><li>quickly - Polls quickly until stable configuration is rectrieved</li><li>quickly= - Polls quickly every default minutes.</li><li>quickly=<N> - Polls quickly every *N* minutes</li></ul></li><li>/slowly[=[ <N>]] (Polls slowly)<br /><br /><ul><li>slowly - Polls slowly until stable configuration is retrieved</li><li>slowly= - Polls slowly every default minutes</li><li>slowly=<N> - Polls quickly every *N* minutes</li></ul></li><li>/now (Polls now)</li></ul>`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Dumps the internal tables, thread, and memory information for the NT File Replication Service \(NTFRS\). It runs against local and remote servers. The recovery setting for NTFRS in Service Control Manager \(SCM\) can be critical to locating and keeping IMPORTANT log events on the computer. This tool provides a convenient method of reviewing those settings.   `, `ntfrsutl[idtable|configtable|inlog|outlog][<computer>]  

ntfrsutl[memory|threads|stage][<computer>]  

ntfrsutl ds[<computer>]  

ntfrsutl [sets][<computer>]  

ntfrsutl [version][<computer>]  

ntfrsutl poll[/quickly[=[<N>]]][/slowly[=[<N>]]][/now][<computer>]`, "", () => { }),
    new ConsoleCommand(`openfiles`, [
        new Parameter(`/s <System>`, `Specifies the remote system to connect to (by name or IP address). Do not use backslashes. If you do not use the /s option, the command is executed on the local computer by default. This parameter applies to all files and folders that are specified in the command.`, ``),
        new Parameter(`/u [<Domain>]<UserName>`, `Executes the command by using the permissions of the specified user account. If you do not use the /u option, system permissions are used by default.`, ``),
        new Parameter(`/p [<Password>]`, `Specifies the password of the user account that is specified in the /u option. If you do not use the /p option, a password prompt appears when the command is executed.`, ``),
        new Parameter(`/id <OpenFileID>`, `Disconnects open files by the specified file ID. The wildcard character (*) can be used with this parameter.</br>Note: You can use the openfiles /query command to find the file ID.`, ``),
        new Parameter(`/a <AccessedBy>`, `Disconnects all open files associated with the user name that is specified in the *AccessedBy* parameter. The wildcard character (*) can be used with this parameter.`, ``),
        new Parameter(`/o {read | write | read/write}`, `Disconnects all open files with the specified open mode value. Valid values are Read, Write, or Read/Write. The wildcard character (*) can be used with this parameter.`, ``),
        new Parameter(`/op <OpenFile>`, `Disconnects all open file connections that are created by a specific open file name. The wildcard character (*) can be used with this parameter.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Enables an administrator to query, display, or disconnect files and directories that have been opened on a system. Also enables or disables the system Maintain Objects List global flag.`, `openfiles /disconnect [/s <System> [/u [<Domain>\]<UserName> [/p [<Password>]]]] {[/id <OpenFileID>] | [/a <AccessedBy>] | [/o {read | write | read/write}]} [/op <OpenFile>]`, "", () => { }),
    new ConsoleCommand(`pagefileconfig`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
    ], `pagefileconfig is deprecated, and is not guaranteed to be supported in future releases of Windows.  `, ``, "", () => { }),
    new ConsoleCommand(`path`, [
        new Parameter(`[<Drive>:]<Path>`, `Specifies the drive and directory to set in the command path.`, ``),
        new Parameter(`;`, `Separates directories in the command path. If used without other parameters, ; clears the existing command paths from the PATH environment variable and directs Cmd.exe to search only in the current directory.`, ``),
        new Parameter(`%PATH%`, `Appends the command path to the existing set of directories listed in the PATH environment variable.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Sets the command path in the PATH environment variable (the set of directories used to search for executable files). If used without parameters, path displays the current command path.`, `path [[<Drive>:]<Path>[;...][;%PATH%]]

path ;`, "", () => { }),
    new ConsoleCommand(`pathping`, [
        new Parameter(`/n`, `Prevents pathping from attempting to resolve the IP addresses of intermediate routers to their names. This might expedite the display of pathping results.`, ``),
        new Parameter(`/h <MaximumHops>`, `Specifies the maximum number of hops in the path to search for the target (destination). The default is 30 hops.`, ``),
        new Parameter(`/g <Hostlist>`, `Specifies that the echo Request messages use the Loose Source Route option in the IP header with the set of intermediate destinations specified in *Hostlist*. With loose source routing, successive intermediate destinations can be separated by one or multiple routers. The maximum number of addresses or names in the host list is 9. The *Hostlist* is a series of IP addresses (in dotted decimal notation) separated by spaces.`, ``),
        new Parameter(`/p <Period>`, `Specifies the number of milliseconds to wait between consecutive pings. The default is 250 milliseconds (1/4 second).`, ``),
        new Parameter(`/q <NumQueries>`, `Specifies the number of echo Request messages sent to each router in the path. The default is 100 queries.`, ``),
        new Parameter(`/w <timeout>`, `Specifies the number of milliseconds to wait for each reply. The default is 3000 milliseconds (3 seconds).`, ``),
        new Parameter(`/i <IPaddress>`, `Specifies the source address.`, ``),
        new Parameter(`/4 <IPv4>`, `Specifies that pathping uses IPv4 only.`, ``),
        new Parameter(`/6 <IPv6>`, `Specifies that pathping uses IPv6 only.`, ``),
        new Parameter(`<TargetName>`, `Specifies the destination, which is identified either by IP address or host name.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Provides information about network latency and network loss at intermediate hops between a source and destination. pathping sends multiple echo Request messages to each router between a source and destination over a period of time and then computes results based on the packets returned from each router. Because pathping displays the degree of packet loss at any given router or link, you can determine which routers or subnets might be having network problems. `, `pathping [/n] [/h] [/g <Hostlist>] [/p <Period>] [/q <NumQueries> [/w <timeout>] [/i <IPaddress>] [/4 <IPv4>] [/6 <IPv6>][<TargetName>]`, "", () => { }),
    new ConsoleCommand(`pause`, [
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Suspends the processing of a batch program and displays the following prompt:`, `Press any key to continue . . .`, "", () => { }),
    new ConsoleCommand(`pbadmin`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Pbadmin is deprecated, and is not guaranteed to be supported in future releases of Windows.`, ``, "", () => { }),
    new ConsoleCommand(`pentnt`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Pentnt is deprecated, and is not guaranteed to be supported in future releases of Windows.`, ``, "", () => { }),
    new ConsoleCommand(`perfmon`, [
        new Parameter(`/res`, `Start Resource View.`, ``),
        new Parameter(`/report`, `Start the System Diagnostics Data Collector Set and display a report of the results.`, ``),
        new Parameter(`/rel`, `Start Reliability Monitor.`, ``),
        new Parameter(`/sys`, `Start Performance Monitor.`, ``),
    ], `Start Windows Reliability and Performance Monitor in a specific standalone mode.`, `perfmon </res|report|rel|sys>`, "", () => { }),
    new ConsoleCommand(`ping`, [
        new Parameter(`/t`, `Specifies that ping continue sending echo Request messages to the destination until interrupted. To interrupt and display statistics, press CTRL+break. To interrupt and quit ping, press CTRL+C.`, ``),
        new Parameter(`/a`, `Specifies that reverse name resolution is performed on the destination IP address. If this is successful, ping displays the corresponding host name.`, ``),
        new Parameter(`/n <Count>`, `Specifies the number of echo Request messages sent. The default is 4.`, ``),
        new Parameter(`/l <Size>`, `Specifies the length, in bytes, of the Data field in the echo Request messages sent. The default is 32. The maximum Size is 65,527.`, ``),
        new Parameter(`/f`, `Specifies that echo Request messages are sent with the Do not Fragment flag in the IP header set to 1 (available on IPv4 only). The echo Request message cannot be fragmented by routers in the path to the destination. This parameter is useful for troubleshooting path Maximum Transmission Unit (PMTU) problems.`, ``),
        new Parameter(`/I <TTL>`, `Specifies the value of the TTL field in the IP header for echo Request messages sent. The default is the default TTL value for the host. The maximum *TTL* is 255.`, ``),
        new Parameter(`/v <TOS>`, `Specifies the value of the type of Service (TOS) field in the IP header for echo Request messages sent (available on IPv4 only). The default is 0. *TOS* is specified as a decimal value from 0 through 255.`, ``),
        new Parameter(`/r <Count>`, `Specifies that the Record Route option in the IP header is used to record the path taken by the echo Request message and corresponding echo Reply message (available on IPv4 only). Each hop in the path uses an entry in the Record Route option. If possible, specify a *Count* that is equal to or greater than the number of hops between the source and destination. The *Count* must be a minimum of 1 and a maximum of 9.`, ``),
        new Parameter(`/s <Count>`, `Specifies that the Internet timestamp option in the IP header is used to record the time of arrival for the echo Request message and corresponding echo Reply message for each hop. The *Count* must be a minimum of 1 and a maximum of 4. This is required for link-local destination addresses.`, ``),
        new Parameter(`/j <Hostlist>`, `Specifies that the echo Request messages use the Loose Source Route option in the IP header with the set of intermediate destinations specified in *Hostlist* (available on IPv4 only). With loose source routing, successive intermediate destinations can be separated by one or multiple routers. The maximum number of addresses or names in the host list is 9. The host list is a series of IP addresses (in dotted decimal notation) separated by spaces.`, ``),
        new Parameter(`/k <Hostlist>`, `Specifies that the echo Request messages use the Strict Source Route option in the IP header with the set of intermediate destinations specified in *Hostlist* (available on IPv4 only). With strict source routing, the next intermediate destination must be directly reachable (it must be a neighbor on an interface of the router). The maximum number of addresses or names in the host list is 9. The host list is a series of IP addresses (in dotted decimal notation) separated by spaces.`, ``),
        new Parameter(`/w <timeout>`, `Specifies the amount of time, in milliseconds, to wait for the echo Reply message that corresponds to a given echo Request message to be received. If the echo Reply message is not received within the time-out, the "Request timed out" error message is displayed. The default time-out is 4000 (4 seconds).`, ``),
        new Parameter(`/R`, `Specifies that the round-trip path is traced (available on IPv6 only).`, ``),
        new Parameter(`/S <Srcaddr>`, `Specifies the source address to use (available on IPv6 only).`, ``),
        new Parameter(`/4`, `Specifies that IPv4 is used to ping. This parameter is not required to identify the target host with an IPv4 address. It is only required to identify the target host by name.`, ``),
        new Parameter(`/6`, `Specifies that IPv6 is used to ping. This parameter is not required to identify the target host with an IPv6 address. It is only required to identify the target host by name.`, ``),
        new Parameter(`<TargetName>`, `Specifies the host name or IP address of the destination.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `The ping command verifies IP-level connectivity to another TCP/IP computer by sending Internet Control Message Protocol (ICMP) echo Request messages. The receipt of corresponding echo Reply messages are displayed, along with round-trip times. ping is the primary TCP/IP command used to troubleshoot connectivity, reachability, and name resolution. Used without parameters,  ping displays help.`, `ping [/t] [/a] [/n <Count>] [/l <Size>] [/f] [/I <TTL>] [/v <TOS>] [/r <Count>] [/s <Count>] [{/j <Hostlist> | /k <Hostlist>}] [/w <timeout>] [/R] [/S <Srcaddr>] [/4] [/6] <TargetName>`, "", () => { }),
    new ConsoleCommand(`pnpunattend`, [
        new Parameter(`auditSystem`, `Specifies online driver install.</br>Required, except when pnpunattend is run with either the /Help or /? parameters.`, ``),
        new Parameter(`/s`, `Optional. Specifies to search for drivers without installing.`, ``),
        new Parameter(`/L`, `Optional. Specifies to display the log information for this command in the command prompt.`, ``),
        new Parameter(`/?`, `Optional. Displays help for this command at the command prompt.`, ``),
    ], `Audits a computer for device drivers, and perform unattended driver installations, or search for drivers without installing and, optionally, report the results to the command line. Use this command to specify the installation of specific drivers for specific hardware devices. See Remarks.`, `PnPUnattend.exe auditSystem [/help] [/?] [/h] [/s] [/L]`, "", () => { }),
    new ConsoleCommand(`pnputil`, [
        new Parameter(`-a`, `Specifies to add the identified INF file.`, ``),
        new Parameter(`-d`, `Specifies to delete the identified INF file.`, ``),
        new Parameter(`-e`, `Specifies to enumerate all third-party INF files.`, ``),
        new Parameter(`-f`, `Specifies to force the deletion of the identified INF file. Cannot be used in conjunction with the â€“i parameter.`, ``),
        new Parameter(`-i`, `Specifies to install the identified INF file. Cannot be used in conjunction with  the -f parameter.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Pnputil.exe is a command line utility that you can use to manage the driver store. You can use Pnputil to add driver packages, remove driver packages, and list driver packages that are in the store.`, `pnputil.exe [-f | -i] [ -? | -a | -d | -e ] <INF name>`, "", () => { }),
    new ConsoleCommand(`popd`, [
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Changes the current directory to the directory that was most recently stored by the pushd command.`, `popd`, "", () => { }),
    new ConsoleCommand(`PowerShell`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Windows PowerShell is a task-based command-line shell and scripting language designed especially for system administration. Built on the .NET Framework, Windows PowerShell helps IT professionals and power users control and automate the administration of the Windows operating system and applications that run on Windows.`, `PowerShell.exe -ExecutionPolicy Restricted`, "", () => { }),
    new ConsoleCommand(`PowerShell_ise`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Windows PowerShell Integrated Scripting Environment (ISE) is a graphical host application that enables you to read, write, run, debug, and test scripts and modules in a graphic-assisted environment. Key features such as IntelliSense, Show-Command, snippets, tab completion, syntax-coloring, visual debugging, and context-sensitive Help provide a rich scripting experience.`, `PowerShell_Ise`, "", () => { }),
    new ConsoleCommand(`print`, [
        new Parameter(`/d:<PrinterName>`, `Specifies the printer that you want to print the job. To print to a locally connected printer, specify the port on your computer where the printer is connected.</br>-   Valid values for parallel ports are LPT1, LPT2, and LPT3.</br>-   Valid values for serial ports are COM1, COM2, COM3, and COM4.</br>You can also specify a network printer by using its queue name (\\\*ServerNamePrinterName*). If you do not specify a printer, the print job is sent to LPT1 by default.`, ``),
        new Parameter(`<Drive>:`, `Specifies the logical or physical drive where the file you want to print is located. This parameter is not required if the file you want to print is located on the current drive.`, ``),
        new Parameter(`<Path>`, `Specifies the location of the file you want to print. This parameter is not required if the file you want to print is located in the current directory.`, ``),
        new Parameter(`<FileName>[ ...]`, `Required. Specifies the file you want to print. You can include multiple files in one command.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Sends a text file to a printer.`, `Print [/d:<PrinterName>] [<Drive>:][<Path>]<FileName>[ ...]`, "", () => { }),
    new ConsoleCommand(`prncnfg`, [
        new Parameter(`-g`, `Displays configuration information about a printer.`, ``),
        new Parameter(`-t`, `Configures a printer.`, ``),
        new Parameter(`-x`, `renames a printer.`, ``),
        new Parameter(`-S <ServerName>`, `Specifies the name of the remote computer that hosts the printer that you want to manage. If you do not specify a computer, the local computer is used.`, ``),
        new Parameter(`-P <printerName>`, `Specifies the name of the printer that you want to manage. Required.`, ``),
        new Parameter(`-z <NewprinterName>`, `Specifies the new printer name. Requires the -x and -P parameters.`, ``),
        new Parameter(`-u <UserName> -w <Password>`, `Specifies an account with permissions to connect to the computer that hosts the printer that you want to manage. All members of the target computer's local Administrators group have these permissions, but the permissions can also be granted to other users. If you do not specify an account, you must be logged on under an account with these permissions for the command to work.`, ``),
        new Parameter(`-r <PortName>`, `Specifies the port where the printer is connected. If this is a parallel or a serial port, then use the ID of the port (for example, LPT1 or COM1). If this is a TCP/IP port, use the port name that was specified when the port was added.`, ``),
        new Parameter(`-l <Location>`, `Specifies the printer location, such as "copy Room."`, ``),
        new Parameter(`-h <Sharename>`, `Specifies the printer's share name.`, ``),
        new Parameter(`-m <Comment>`, `Specifies the printer's comment string.`, ``),
        new Parameter(`-f <SeparatorFileName>`, `Specifies a file that contains the text that appears on the separator page.`, ``),
        new Parameter(`-y <Datatype>`, `Specifies the data types that the printer can accept.`, ``),
        new Parameter(`-st <starttime>`, `Configures the printer for limited availability. Specifies the time of day the printer is available. If you send a document to a printer when it is unavailable, the document is held (spooled) until the printer becomes available. You must specify time as a 24-hour clock. For example, to specify 11:00 P.M., type 2300.`, ``),
        new Parameter(`-ut <Endtime>`, `Configures the printer for limited availability. Specifies the time of day the printer is no longer available. If you send a document to a printer when it is unavailable, the document is held (spooled) until the printer becomes available. You must specify time as a 24-hour clock. For example, to specify 11:00 P.M., type 2300.`, ``),
        new Parameter(`-o <Priority>`, `Specifies a priority that the spooler uses to route print jobs into the print queue. A print queue with a higher priority receives all its jobs before any queue with a lower priority.`, ``),
        new Parameter(`-i <DefaultPriority>`, `Specifies the default priority assigned to each print job.`, ``),
        new Parameter(`{+&#124;-}shared`, `Specifies whether this printer is shared on the network.`, ``),
        new Parameter(`{+&#124;-}direct`, `Specifies whether the document should be sent directly to the printer without being spooled.`, ``),
        new Parameter(`{+&#124;-}published`, `Specifies whether this printer should be published in active directory. If you publish the printer, other users can search for it based on its location and capabilities (such as color printing and stapling).`, ``),
        new Parameter(`{+&#124;-}hidden`, `Reserved function.`, ``),
        new Parameter(`{+&#124;-}rawonly`, `Specifies whether only raw data print jobs can be spooled in this queue.`, ``),
        new Parameter(`{+ &#124; -}queued`, `Specifies that the printer should not begin to print until after the last page of the document is spooled. The printing program is unavailable until the document has finished printing. However, using this parameter ensures that the whole document is available to the printer.`, ``),
        new Parameter(`{+ &#124; -}keepprintedjobs`, `Specifies whether the spooler should retain documents after they are printed. Enabling this option allows a user to resubmit a document to the printer from the print queue instead of from the printing program.`, ``),
        new Parameter(`{+ &#124; -}workoffline`, `Specifies whether a user is able to send print jobs to the print queue if the computer is not connected to the network.`, ``),
        new Parameter(`{+ &#124; -}enabledevq`, `Specifies whether print jobs that do not match the printer setup (for example, PostScript files spooled to non-PostScript printers) should be held in the queue rather than being printed.`, ``),
        new Parameter(`{+ &#124; -}docompletefirst`, `Specifies whether the spooler should send print jobs with a lower priority that have completed spooling before sending print jobs with a higher priority that have not completed spooling. If this option is enabled and no documents have completed spooling, the spooler will send larger documents before smaller ones. You should enable this option if you want to maximize printer efficiency at the cost of job priority. If this option is disabled, the spooler always sends higher priority jobs to their respective queues first.`, ``),
        new Parameter(`{+ &#124; -}enablebidi`, `Specifies whether the printer sends status information to the spooler.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Configures or displays configuration information about a printer.`, `cscript Prncnfg {-g | -t | -x | -?} [-S <ServerName>] [-P <printerName>] [-z <NewprinterName>] [-u <UserName>] [-w <Password>] [-r <PortName>] [-l <Location>] [-h <Sharename>] [-m <Comment>] [-f <SeparatorFileName>] [-y <Datatype>] [-st <starttime>] [-ut <Untiltime>] [-i <DefaultPriority>] [-o <Priority>] [<+|->shared] [<+|->direct] [<+|->hidden] [<+|->published] [<+|->rawonly] [<+|->queued] [<+|->enablebidi] [<+|->keepprintedjobs] [<+|->workoffline] [<+|->enabledevq] [<+|->docompletefirst]`, "", () => { }),
    new ConsoleCommand(`prndrvr`, [
        new Parameter(`-a`, `Installs a driver.`, ``),
        new Parameter(`-d`, `deletes a driver.`, ``),
        new Parameter(`-l`, `lists all printer drivers installed on the server specified by the -s parameter. If you do not specify a server, Windows lists the printer drivers installed on the local computer.`, ``),
        new Parameter(`-x`, `deletes all printer drivers and additional printer drivers not in use by a logical printer on the server specified by the -s parameter. If you do not specify a server to remove from the list, Windows deletes all unused printer drivers on the local computer.`, ``),
        new Parameter(`-m <DrivermodelName>`, `Specifies (by name) the driver you want to install. Drivers are often named for the model of printer they support. See the printer documentation for more information.`, ``),
        new Parameter(`-v {0 &#124; 1 &#124; 2 &#124; 3}`, `Specifies the version of the driver you want to install. See the description of the -eparameter for information on which versions are available for which environment. If you do not specify a version, the version of the driver appropriate for the version of Windows running on the computer where you are installing the driver is installed.<br /><br />-   version 0 supports Windows 95, Windows 98, and Windows Millennium edition.<br />-   version 1 supports Windows NT 3.51.<br />-   version 2 supports Windows NT 4.0.<br />-   version 3 supports Windows Vista, Windows XP, Windows 2000, and the Windows Server 2003 operating systems. Note that this is the only printer driver version that Windows Vista supports.`, ``),
        new Parameter(`-e <Environment>`, `Specifies the environment for the driver you want to install. If you do not specify an environment, the environment of the computer where you are installing the driver is used. The supported environment parameters are:<br /><br />-   "Windows NT x86"<br />-   "Windows x64"<br />-   "Windows IA64"`, ``),
        new Parameter(`-s <ServerName>`, `Specifies the name of the remote computer that hosts the printer that you want to manage. If you do not specify a computer, the local computer is used.`, ``),
        new Parameter(`-u <UserName> -w <Password>`, `Specifies an account with permissions to connect to the computer that hosts the printer that you want to manage. All members of the target computer's local Administrators group have these permissions, but the permissions can also be granted to other users. If you do not specify an account, you must be logged on under an account with these permissions for the command to work.`, ``),
        new Parameter(`-h <path>`, `Specifies the path to the driver file. If you do not specify a path, the path to the location where Windows was installed is used.`, ``),
        new Parameter(`-i <Filename.inf>`, `Specifies the complete path and file name for the driver you want to install. If you do not specify a file name, the script uses one of the inbox printer .inf files in the inf subdirectory of the Windows directory.<br /><br />if the driver path is not specified, the script searches for driver files in the driver.cab file.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Use the prndrvr command to add, delete, and list printer drivers.`, `cscript prndrvr {-a | -d | -l | -x | -?} [-m <model>] [-v {0|1|2|3}] 

[-e <environment>] [-s <ServerName>] [-u <UserName>] [-w <Password>] 

[-h <path>] [-i <inf file>]`, "", () => { }),
    new ConsoleCommand(`prnjobs`, [
        new Parameter(`-z`, `pauses the print job specified with the -j parameter.`, ``),
        new Parameter(`-m`, `Resumes the print job specified with the -j parameter.`, ``),
        new Parameter(`-x`, `Cancels the print job specified with the -j parameter.`, ``),
        new Parameter(`-l`, `lists all the print jobs in a print queue.`, ``),
        new Parameter(`-s <ServerName>`, `Specifies the name of the remote computer that hosts the printer that you want to manage. If you do not specify a computer, the local computer is used.`, ``),
        new Parameter(`-p <printerName>`, `Specifies the name of the printer that you want to manage. Required.`, ``),
        new Parameter(`-j <JobID>`, `Specifies (by ID number) the print job you want to cancel.`, ``),
        new Parameter(`-u <UserName> -w <Password>`, `Specifies an account with permissions to connect to the computer that hosts the printer that you want to manage. All members of the target computer's local Administrators group have these permissions, but the permissions can also be granted to other users. If you do not specify an account, you must be logged on under an account with these permissions for the command to work.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Pauses, resumes, cancels, and lists print jobs.`, `cscript Prnjobs {-z | -m | -x | -l | -?} [-s <ServerName>] 

[-p <printerName>] [-j <JobID>] [-u <UserName>] [-w <Password>]`, "", () => { }),
    new ConsoleCommand(`prnmngr`, [
        new Parameter(`-a`, `adds a local printer connection.`, ``),
        new Parameter(`-d`, `deletes a printer connection.`, ``),
        new Parameter(`-x`, `deletes all printers from the server specified with the -s parameter. If you do not specify a server, Windows deletes all printers on the local computer.`, ``),
        new Parameter(`-g`, `Displays the default printer.`, ``),
        new Parameter(`-t`, `Sets the default printer to the printer specified by the -p parameter.`, ``),
        new Parameter(`-l`, `lists all printers installed on the server specified by the -s parameter. If you do not specify a server, Windows lists the printers installed on the local computer.`, ``),
        new Parameter(`c`, `Specifies that the parameter applies to printer connections. Can be used with the -a and -x parameters.`, ``),
        new Parameter(`-s <ServerName>`, `Specifies the name of the remote computer that hosts the printer that you want to manage. If you do not specify a computer, the local computer is used.`, ``),
        new Parameter(`-p <printerName>`, `Specifies the name of the printer that you want to manage.`, ``),
        new Parameter(`-m <DrivermodelName>`, `Specifies (by name) the driver you want to install. Drivers are often named for the model of printer they support. See the printer documentation for more information.`, ``),
        new Parameter(`-r <PortName>`, `Specifies the port where the printer is connected. If this is a parallel or a serial port, use the ID of the port (for example, LPT1: or COM1:). If this is a TCP/IP port, use the port name that was specified when the port was added.`, ``),
        new Parameter(`-u <UserName> -w <Password>`, `Specifies an account with permissions to connect to the computer that hosts the printer that you want to manage. All members of the target computer's local Administrators group have these permissions, but the permissions can also be granted to other users. If you do not specify an account, you must be logged on under an account with these permissions for the command to work.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Adds, deletes, and lists printers or printer connections, in addition to setting and displaying the default printer.`, `cscript Prnmngr {-a | -d | -x | -g | -t | -l | -?}[c] [-s <ServerName>] 

[-p <printerName>] [-m <printermodel>] [-r <PortName>] [-u <UserName>] 

[-w <Password>]`, "", () => { }),
    new ConsoleCommand(`prnport`, [
        new Parameter(`-a`, `creates a standard TCP/IP printer port.`, ``),
        new Parameter(`-d`, `deletes a standard TCP/IP printer port.`, ``),
        new Parameter(`-l`, `lists all standard TCP/IP printer ports on the computer specified with the -s parameter.`, ``),
        new Parameter(`-g`, `Displays the configuration of a standard TCP/IP printer port.`, ``),
        new Parameter(`-t`, `Configures the port settings for a standard TCP/IP printer port.`, ``),
        new Parameter(`-r <PortName>`, `Specifies the port to which the printer is connected.`, ``),
        new Parameter(`-s <ServerName>`, `Specifies the name of the remote computer that hosts the printer that you want to manage. If you do not specify a computer, the local computer is used.`, ``),
        new Parameter(`-u <UserName> -w <Password>`, `Specifies an account with permissions to connect to the computer that hosts the printer that you want to manage. All members of the target computer's local Administrators group have these permissions, but the permissions can also be granted to other users. If you do not specify an account, you must be logged on under an account with these permissions for the command to work.`, ``),
        new Parameter(`-o {raw &#124; lpr}`, `Specifies which protocol the port uses: TCP raw or TCP lpr. If you use TCP raw, you can optionally specify the port number by using the -n parameter. The default port number is 9100.`, ``),
        new Parameter(`-h <Hostaddress>`, `Specifies (by IP address) the printer for which you want to configure the port.`, ``),
        new Parameter(`-q <QueueName>`, `Specifies the queue name for a TCP raw port.`, ``),
        new Parameter(`-n <PortNumber>`, `Specifies the port number for a TCP raw port. The default port number is 9100.`, ``),
        new Parameter(`-m{e &#124; d}`, `Specifies whether SNMP is enabled. The parameter e enables SNMP. The parameter d disables SNMP.`, ``),
        new Parameter(`-i <SNMPIndex`, `Specifies the SNMP index, if SNMP is enabled. For more information, see Rfc 1759 at the [Rfc editor Web site](https://go.microsoft.com/fwlink/?LinkId=569).`, ``),
        new Parameter(`-y <CommunityName>`, `Specifies the SNMP community name, if SNMP is enabled.`, ``),
        new Parameter(`-2{e &#124; -d}`, `Specifies whether double spools (also known as respooling) are enabled for TCP lpr ports. Double spools are necessary because TCP lpr must include an accurate byte count in the control file that is sent to the printer, but the protocol cannot get the count from the local print provider. Therefore, when a file is spooled to a TCP lpr print queue, it is also spooled as a temporary file in the system32 directory. TCP lpr determines the size of the temporary file and sends the size to the server running LPD. The parameter e enables double spools. The parameter d disables double spools.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Creates, deletes, and lists standard TCP/IP printer ports, in addition to displaying and changing port configuration.`, `cscript prnport {-a | -d | -l | -g | -t | -?} [-r <PortName>] 

[-s <ServerName>] [-u <UserName>] [-w <Password>] [-o {raw | lpr}] 

[-h <Hostaddress>] [-q <QueueName>] [-n <PortNumber>] -m{e | d} 

[-i <SNMPIndex>] [-y <CommunityName>] -2{e | -d}`, "", () => { }),
    new ConsoleCommand(`prnqctl`, [
        new Parameter(`-z`, `pauses printing on the printer specified with the -p parameter.`, ``),
        new Parameter(`-m`, `Resumes printing on the printer specified with the -p parameter.`, ``),
        new Parameter(`-e`, `prints a test page on the printer specified with the -p parameter.`, ``),
        new Parameter(`-x`, `Cancels all print jobs on the printer specified with the -p parameter.`, ``),
        new Parameter(`-s <ServerName>`, `Specifies the name of the remote computer that hosts the printer that you want to manage. If you do not specify a computer, the local computer is used.`, ``),
        new Parameter(`-p <printerName>`, `Specifies the name of the printer that you want to manage. Required.`, ``),
        new Parameter(`-u <UserName> -w <Password>`, `Specifies an account with permissions to connect to the computer that hosts the printer that you want to manage. All members of the target computer's local Administrators group have these permissions, but the permissions can also be granted to other users. If you do not specify an account, you must be logged on under an account with these permissions for the command to work.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Prints a test page, pauses or resumes a printer, and clears a printer queue.  `, `cscript Prnqctl {-z | -m | -e | -x | -?} [-s <ServerName>]   

[-p <printerName>] [-u <UserName>] [-w <Password>]`, "", () => { }),
    new ConsoleCommand(`prompt`, [
        new Parameter(`<Text>`, `Specifies the text and information that you want to include in the command prompt.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Changes the Cmd.exe command prompt. If used without parameters, prompt resets the command prompt to the default setting, which is the current drive letter and directory followed by the greater than symbol (>).`, `prompt [<Text>]`, "", () => { }),
    new ConsoleCommand(`pubprn`, [
        new Parameter(`<ServerName>`, `Specifies the name of the Windows server that hosts the printer that you want to publish. If you do not specify a computer, the local computer is used.`, ``),
        new Parameter(`<UNCprinterpath>`, `The Universal Naming Convention (UNC) path to the shared printer that you want to publish.`, ``),
        new Parameter(`"LDAP://CN=<Container>,DC=<Container>"`, `Specifies the path to the container in active directory Domain Services where you want to publish the printer.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Publishes a printer to the active directory Domain Services.`, `cscript pubprn {<ServerName> | <UNCprinterpath>} 

"LDAP://CN=<Container>,DC=<Container>"`, "", () => { }),
    new ConsoleCommand(`pushd`, [
        new Parameter(`<Path>`, `Specifies the directory to make the current directory. This command supports relative paths.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Stores the current directory for use by the popd command, and then changes to the specified directory.`, `pushd [<Path>]`, "", () => { }),
    new ConsoleCommand(`pushprinterconnections`, [
        new Parameter(`<-log>`, `Writes a per user debug log file to %temp, or writes a per machine debug log to %windir%temp.`, ``),
        new Parameter(`<-?>`, `Displays Help at the command prompt.`, ``),
    ], `Reads Deployed Printer Connection settings from Group Policy and deploys/removes printer connections as needed.`, `pushprinterconnections <-log> <-?>`, "", () => { }),
    new ConsoleCommand(`qappsrv`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `Displays a list of all Remote Desktop Session Host (RD Session Host) servers on the network.`, ``, "", () => { }),
    new ConsoleCommand(`qprocess`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `Displays information about processes that are running on a Remote Desktop Session Host (RD Session Host) server.`, ``, "", () => { }),
    new ConsoleCommand(`query`, [
        new Parameter(`[query process](query-process.md)`, `Displays information about processes that are running on an rd Session Host server.`, ``),
        new Parameter(`[query session](query-session.md)`, `Displays information about sessions on an rd Session Host server.`, ``),
        new Parameter(`[query termserver](query-termserver.md)`, `Displays a list of all rd Session Host servers on the network.`, ``),
        new Parameter(`[query user](query-user.md)`, `Displays information about user sessions on an rd Session Host server.`, ``),
    ], `Displays information about processes, sessions, and Remote Desktop Session Host (RD Session Host) servers.`, `query process

query session

query termserver

query user`, "", () => { }),
    new ConsoleCommand(`quser`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
    ], `Displays information about user sessions on a Remote Desktop Session Host (rd Session Host) server.  `, ``, "", () => { }),
    new ConsoleCommand(`qwinsta`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `Displays information about sessions on a Remote Desktop Session Host (RD Session Host) server.`, ``, "", () => { }),
    new ConsoleCommand(`rcp`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Copies files between computers. This command has been deprecated. You can install the Subsystem for UNIX-based Applications using the Add Features Wizard. For more information, see [Windows Server 2008 UNIX Interoperability Components](https://go.microsoft.com/fwlink/?LinkId=191835) at the Microsoft Web site. After installation, you can then open a C Shell or Korn Shell command window and run rcp. For more information, type man rcp at the C Shell or Korn Shell prompt.`, ``, "", () => { }),
    new ConsoleCommand(`rd`, [
        new Parameter(`[<Drive>:]<Path>`, `Specifies the location and the name of the directory that you want to delete. *Path* is required.`, ``),
        new Parameter(`/s`, `Deletes a directory tree (the specified directory and all its subdirectories, including all files).`, ``),
        new Parameter(`/q`, `Specifies quiet mode. Does not prompt for confirmation when deleting a directory tree. (Note that /q works only if /s is specified.)`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Deletes a directory. This command is the same as the rmdir command.`, `rd [<Drive>:]<Path> [/s [/q]]

rmdir [<Drive>:]<Path> [/s [/q]]`, "", () => { }),
    new ConsoleCommand(`rdpsign`, [
        new Parameter(`/sha1 <hash>`, `Specifies the thumbprint, which is the Secure Hash Algorithm 1 (SHA1) hash of the signing certificate that is included in the certificate store.`, ``),
        new Parameter(`/q`, `Quiet mode. No output when the command succeeds and minimal output if the command fails.`, ``),
        new Parameter(`/v`, `verbose mode. Displays all warnings, messages, and status.`, ``),
        new Parameter(`/l`, `Tests the signing and output results without actually replacing any of the input files.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Enables you to digitally sign a Remote Desktop Protocol (.rdp) file.`, `rdpsign /sha1 <hash> [/q | /v |] [/l] <file_name.rdp>`, "", () => { }),
    new ConsoleCommand(`recover`, [
        new Parameter(`[<Drive>:][<Path>]<FileName>`, `Specifies the location and name of the file that you want to recover. *FileName* is required.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Recovers readable information from a bad or defective disk.`, `recover [<Drive>:][<Path>]<FileName>`, "", () => { }),
    new ConsoleCommand(`reg`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Performs operations on registry subkey information and values in registry entries. The reg commands include:`, ``, "", () => { }),
    new ConsoleCommand(`regini`, [
        new Parameter(`-h <hivefile hiveroot>`, `Specifies the local registry hive to modify. You must specify the name of the hive file and the root of the hive in the format hivefile hiveroot.`, ``),
        new Parameter(`-i <n>`, `Specifies the level of indentation to use to indicate the tree structure of registry keys in the command output. The Regdmp.exe tool (which gets a registry keyâ€™s current permissions in binary format) uses indentation in multiples of four, so the default value is 4.`, ``),
        new Parameter(`-o <outputwidth>`, `Specifies the width of the command output, in characters. If the output will appear in the command window, the default value is the width of the window. If the output is directed to a file, the default value is 240 characters.`, ``),
        new Parameter(`-b`, `Specifies that Regini.exe output is backward compatible with previous versions of Regini.exe. See the Remarks section for details.`, ``),
        new Parameter(`textfiles`, `Specifies the name of one or more text files that contain registry data. Any number of ANSI or Unicode text files can be listed.`, ``),
    ], `Modifies the registry from the command line or a script, and applies changes that were preset in one or more text files. You can create, modify, or delete registry keys, in addition to modifying the permissions on the registry keys.`, `regini [-m \\machinename | -h hivefile hiveroot][-i n] [-o outputWidth][-b] textFiles...`, "", () => { }),
    new ConsoleCommand(`regsvr32`, [
        new Parameter(`/u`, `Unregisters server.`, ``),
        new Parameter(`/s`, `Runs Regsvr32 without displaying messages.`, ``),
        new Parameter(`/n`, `Runs Regsvr32 without calling DllRegisterServer. (Requires the /i parameter.)`, ``),
        new Parameter(`/i:<cmdline>`, `Passes an optional command-line string (*cmdline*) to DllInstall. If you use this parameter in conjunction with the /u parameter, it calls DllUninstall.`, ``),
        new Parameter(`<DllName>`, `The name of the .dll file that will be registered.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Registers .dll files as command components in the registry.`, `regsvr32 [/u] [/s] [/n] [/i[:cmdline]] <DllName>`, "", () => { }),
    new ConsoleCommand(`relog`, [
        new Parameter(`*FileName* [*FileName ...*]`, `Specifies the pathname of an existing performance counter log. You can specify multiple input files.`, ``),
        new Parameter(`-a `, `Appends output file instead of overwriting. This option does not apply to SQL format where the default is always to append.  `, ``),
        new Parameter(`-c *path* [*path ...*]`, `Specifies the performance counter path to log. To specify multiple counter paths, separate them with a space and enclose the counter paths in quotation marks (for example, "*Counterpath1* *Counterpath2*")`, ``),
        new Parameter(`-cf *FileName*`, `Specifies the pathname of the text file that lists the performance counters to be included in a relog file. Use this option to list counter paths in an input file, one per line. Default setting is all counters in the original log file are relogged.`, ``),
        new Parameter(`-f {bin| csv|tsv|SQL}`, `Specifies the pathname of the output file format. The default format is bin. For a SQL database, the output file specifies the *DSN!CounterLog*. You can specify the database location by using the ODBC manager to configure the DSN (Database System Name).  `, ``),
        new Parameter(`-t *Value*`, `Specifies sample intervals in "*N*" records. Includes every nth data point in the relog file. Default is every data point.`, ``),
        new Parameter(`-o {*OutputFile* | *"SQL:DSN!Counter_Log*} where DSN is a ODMC DSN defined on the system.`, `Specifies the pathname of the output file or SQL database where the counters will be written. <br>Note: For the 64-bit and 32-bit versions of Relog.exe, you need to define a DSN in the ODBC Data Source (64-bit and 32-bit respectively)`, ``),
        new Parameter(`-b <*M*/*D*/*YYYY*> [[*HH*:]*MM*:]*SS*`, `Specifies begin time for copying first record from the input file. date and time must be in this exact format *M*/*D*/*YYYYHH*:*MM*:*SS*.`, ``),
        new Parameter(`-e <*M*/*D*/*YYYY*> [[*HH*:]*MM*:]*SS* `, `Specifies end time for copying last record from the input file. date and time must be in this exact format *M*/*D*/*YYYYHH*:*MM*:*SS*.`, ``),
        new Parameter(`-config {*FileName* | *i*}`, `Specifies the pathname of the settings file that contains command-line parameters. Use *-i* in the configuration file as a placeholder for a list of input files that can be placed on the command line. On the command line, however, you do not need to use *i*. You can also use wildcards such as *.blg to specify many input file names.`, ``),
        new Parameter(`-q`, `Displays the performance counters and time ranges of log files specified in the input file.`, ``),
        new Parameter(`-y`, `Bypasses prompting by answering "yes" to all questions.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Extracts performance counters from performance counter logs into other formats, such as text-TSV (for tab-delimited text), text-CSV (for comma-delimited text), binary-BIN, or SQL.   `, `relog [<FileName> [<FileName> ...]] [/a] [/c <path> [<path> ...]] [/cf <FileName>] [/f  {bin|csv|tsv|SQL}] [/t <Value>] [/o {OutputFile|DSN!CounterLog}] [/b <M/D/YYYY> [[<HH>:] <MM>:] <SS>] [/e <M/D/YYYY> [[<HH>:] <MM>:] <SS>] [/config {<FileName>|i}] [/q]`, "", () => { }),
    new ConsoleCommand(`rem`, [
        new Parameter(`<Comment>`, `Specifies a string of characters to include as a comment.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Records comments (remarks) in a batch file or CONFIG.SYS. If no comment is specified, rem adds vertical spacing.`, `rem [<Comment>]`, "", () => { }),
    new ConsoleCommand(`ren`, [
        new Parameter(`[<Drive>:][<Path>]<FileName1>`, `Specifies the location and name of the file or set of files you want to rename. *FileName1* can include wildcard characters (* and ?).`, ``),
        new Parameter(`<FileName2>`, `Specifies the new name for the file. You can use wildcard characters to specify new names for multiple files.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Renames files or directories. This command is the same as the rename command.`, `ren [<Drive>:][<Path>]<FileName1> <FileName2>

rename [<Drive>:][<Path>]<FileName1> <FileName2>`, "", () => { }),
    new ConsoleCommand(`rename`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `This is the same as the ren command.`, ``, "", () => { }),
    new ConsoleCommand(`repair-bde`, [
        new Parameter(`<InputVolume>`, `Identifies the drive letter of the BitLocker-encrypted drive that you want to repair. The drive letter must include a colon; for example: C:.`, ``),
        new Parameter(`<OutputVolumeorImage>`, `Identifies the drive on which to store the content of the repaired drive. All information on the output drive will be overwritten.`, ``),
        new Parameter(`-rk`, `Identifies the location of the recovery key that should be used to unlock the volume. This command may also be specified as -recoverykey.`, ``),
        new Parameter(`-rp`, `Identifies the numerical recovery password that should be used to unlock the volume. This command may also be specified as -recoverypassword.`, ``),
        new Parameter(`-pw`, `Identifies the password that should be used to unlock the volume. This command may also be specified as -password`, ``),
        new Parameter(`-kp`, `Identifies the recovery key package that can be used to unlock the volume. This command may also be specified as -keypackage.`, ``),
        new Parameter(`-lf`, `Specifies the path to the file that will store Repair-bde error, warning, and information messages. This command may also be specified as -logfile.`, ``),
        new Parameter(`-f`, `Forces a volume to be dismounted even if it cannot be locked. This command may also be specified as -force.`, ``),
        new Parameter(`-? or /?`, `Displays Help at the command prompt.`, ``),
    ], `Accesses encrypted data on a severely damaged hard disk if the drive was encrypted by using BitLocker. Repair-bde can reconstruct critical parts of the drive and salvage recoverable data as long as a valid recovery password or recovery key is used to decrypt the data. If the BitLocker metadata data on the drive has become corrupt, you must be able to supply a backup key package in addition to the recovery password or recovery key. This key package is backed up in Active Directory Domain Services (AD DS) if you used the default setting for AD DS backup. With this key package and either the recovery password or recovery key, you can decrypt portions of a BitLocker-protected drive if the disk is corrupted. Each key package will work only for a drive that has the corresponding drive identifier. You can use the [BitLocker Recovery Password Viewer for Active Directory](https://technet.microsoft.com/library/dd875531(v=ws.10).aspx) to obtain this key package from AD DS.`, `repair-bde <InputVolume> <OutputVolumeorImage> [-rk] [â€“rp] [-pw] [â€“kp] [â€“lf] [-f] [{-?|/?}]`, "", () => { }),
    new ConsoleCommand(`replace`, [
        new Parameter(`[<Drive1>:][<Path1>]<FileName>`, `Specifies the location and name of the source file or set of files. *FileName* is required, and can include wildcard characters (* and ?).`, ``),
        new Parameter(`[<Drive2>:][<Path2>]`, `Specifies the location of the destination file. You cannot specify a file name for files you replace. If you do not specify a drive or path, replace uses the current drive and directory as the destination.`, ``),
        new Parameter(`/a`, `Adds new files to the destination directory instead of replacing existing files. You cannot use this command-line option with the /s or /u command-line option.`, ``),
        new Parameter(`/p`, `Prompts you for confirmation before replacing a destination file or adding a source file.`, ``),
        new Parameter(`/r`, `Replaces Read-only and unprotected files. If you attempt to replace a Read-only file, but you do not specify /r, an error results and stops the replacement operation.`, ``),
        new Parameter(`/w`, `Waits for you to insert a disk before the search for source files begins. If you do not specify /w, replace begins replacing or adding files immediately after you press ENTER.`, ``),
        new Parameter(`/s`, `Searches all subdirectories in the destination directory and replaces matching files. You cannot use /s with the /a command-line option. The replace command does not search subdirectories that are specified in *Path1*.`, ``),
        new Parameter(`/u`, `Replaces only those files on the destination directory that are older than those in the source directory. You cannot use /u with the /a command-line option.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Replaces files. If used with the /a option, replace adds new files to a directory instead of replacing existing files.`, `replace [<Drive1>:][<Path1>]<FileName> [<Drive2>:][<Path2>] [/a] [/p] [/r] [/w] 

replace [<Drive1>:][<Path1>]<FileName> [<Drive2>:][<Path2>] [/p] [/r] [/s] [/w] [/u]`, "", () => { }),
    new ConsoleCommand(`rexec`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Rexec is deprecated, and is not guaranteed to be supported in future releases of Windows.`, ``, "", () => { }),
    new ConsoleCommand(`risetup`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `The risetup command is deprecated in Windows ServerÂ® 2008 and Windows Server 2008 R2.`, ``, "", () => { }),
    new ConsoleCommand(`rmdir`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `This command is the same as the rd command. See [Rd](rd.md) for syntax and parameters.`, ``, "", () => { }),
    new ConsoleCommand(`robocopy`, [
        new Parameter(`<Source>`, `Specifies the path to the source directory.`, ``),
        new Parameter(`<Destination>`, `Specifies the path to the destination directory.`, ``),
        new Parameter(`<File>`, `Specifies the file or files to be copied. You can use wildcard characters (* or ?), if you want. If the File parameter is not specified, *.* is used as the default value.`, ``),
        new Parameter(`<Options>`, `Specifies options to be used with the robocopy command.`, ``),
    ], `Copies file data.`, `robocopy <Source> <Destination> [<File>[ ...]] [<Options>]`, "", () => { }),
    new ConsoleCommand(`route_ws2008`, [
        new Parameter(`/f`, `Clears the routing table of all entries that are not host routes (routes with a netmask of 255.255.255.255), the loopback network route (routes with a destination of 127.0.0.0 and a netmask of 255.0.0.0), or a multicast route (routes with a destination of 224.0.0.0 and a netmask of 240.0.0.0). If this is used in conjunction with one of the commands (such as add, change, or delete), the table is cleared prior to running the command.`, ``),
        new Parameter(`/p`, `When used with the add command, the specified route is added to the registry and is used to initialize the IP routing table whenever the TCP/IP protocol is started. By default, added routes are not preserved when the TCP/IP protocol is started. When used with the print command, the list of persistent routes is displayed. This parameter is ignored for all other commands. Persistent routes are stored in the registry location HKEY_LOCAL_MACHINESYSTEMCurrentControlSetServicesTcpipParametersPersistentRoutes.`, ``),
        new Parameter(`<Command>`, `Specifies the command you want to run. The following table lists valid commands:<br /><br />-   add: adds a route.<br />-   change: modifies an existing route.<br />-   delete: deletes a route or routes.<br />-   print: prints a route or routes.`, ``),
        new Parameter(`<Destination>`, `Specifies the network destination of the route. The destination can be an IP network address (where the host bits of the network address are set to 0), an IP address for a host route, or 0.0.0.0 for the default route.`, ``),
        new Parameter(`mask <Netmask>`, `Specifies the network destination of the route. The destination can be an IP network address (where the host bits of the network address are set to 0), an IP address for a host route, or 0.0.0.0 for the default route.`, ``),
        new Parameter(`<Gateway>`, `Specifies the forwarding or next hop IP address over which the set of addresses defined by the network destination and subnet mask are reachable. For locally attached subnet routes, the gateway address is the IP address assigned to the interface that is attached to the subnet. For remote routes, available across one or more routers, the gateway address is a directly reachable IP address that is assigned to a neighboring router.`, ``),
        new Parameter(`metric <Metric>`, `Specifies an integer cost metric (ranging from 1 to 9999) for the route, which is used when choosing among multiple routes in the routing table that most closely match the destination address of a packet being forwarded. The route with the lowest metric is chosen. The metric can reflect the number of hops, the speed of the path, path reliability, path throughput, or administrative properties.`, ``),
        new Parameter(`if <Interface>`, `Specifies the interface index for the interface over which the destination is reachable. For a list of interfaces and their corresponding interface indexes, use the display of the route print command. You can use either decimal or hexadecimal values for the interface index. For hexadecimal values, precede the hexadecimal number with 0x. When the if parameter is omitted, the interface is determined from the gateway address.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays and modifies the entries in the local IP routing table. Used without parameters, route displays help.   `, `route [/f] [/p] [<Command> [<Destination>] [mask <Netmask>] [<Gateway>] [metric <Metric>]] [if <Interface>]]`, "", () => { }),
    new ConsoleCommand(`rpcinfo`, [
        new Parameter(`/p [<Node>]`, `lists all programs registered with the port mapper on the specified host. If you do not specify a node (computer) name, the program queries the port mapper on the local host.`, ``),
        new Parameter(`/b <Program version>`, `Requests a response from all network nodes that have the specified program and version registered with the port mapper. You must specify both a program name or number and a version number.`, ``),
        new Parameter(`/t <Node Program> [<version>]`, `Uses the TCP transport protocol to call the specified program. You must specify both a node (computer) name and a program name. If you do not specify a version, the program calls all versions.`, ``),
        new Parameter(`/u <Node Program> [<version>]`, `Uses the UDP transport protocol to call the specified program. You must specify both a node (computer) name and a program name. If you do not specify a version, the program calls all versions.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Lists programs on remote computers. The rpcinfo command-line utility makes a remote procedure call (RPC) to an RPC server and reports what it finds. `, `rpcinfo [/p [<Node>]] [/b <Program version>] [/t <Node Program> [<version>]] [/u <Node Program> [<version>]]`, "", () => { }),
    new ConsoleCommand(`rpcping`, [
        new Parameter(`/t <protseq>`, `Specifies the protocol sequence to use. Can be one of the standard RPC protocol sequences, for example: ncacn_ip_tcp, ncacn_np, or ncacn_http.<br /><br />If not specified, default is ncacn_ip_tcp.`, ``),
        new Parameter(`/s <server_addr>`, `Specifies the server address. If not specified, the local machine will be pinged.`, ``),
        new Parameter(`/e <endpoint>`, `Specifies the endpoint to ping. If none is specified, the endpoint mapper on the target machine will be pinged.<br /><br />This option is mutually exclusive with the interface (/f) option.`, ``),
        new Parameter(`/o <binding_options>`, `Specifies the binding options for the RPC ping.`, ``),
        new Parameter(`/f <interface UUID>[,Majorver]`, `Specifies the interface to ping. This option is mutually exclusive with the endpoint option. The interface is specified as a UUID.<br /><br />if the *Majorver* is not specified, version 1 of the interface will be sought.<br /><br />When interface is specified, rpcping will query the endpoint mapper on the target machine to retrieve the endpoint for the specified interface. The endpoint mapper will be queried using the options specified in the command line.`, ``),
        new Parameter(`/O <Object UUID>`, `Specifies the object UUID if the interface registered one.`, ``),
        new Parameter(`/i <#_iterations>`, `Specifies the number of calls to make. The default is 1. This option is useful for measuring connection latency if multiple iterations are specified.`, ``),
        new Parameter(`/u <security_package_id>`, `Specifies the security package (security provider) RPC will use to make the call. The security package is identified as a number or a name. If a number is used it is the same number as in the RpcBindingSetAuthInfoEx API. The list below shows the names and numbers. Names are not case sensitive:<br /><br />-   Negotiate / 9 or one of nego, snego or negotiate<br />-   NTLM / 10 or NTLM<br />-   SChannel / 14 or SChannel<br />-   Kerberos / 16 or Kerberos<br />-   Kernel / 20 or Kernel<br />    if you specify this option, you must specify authentication level other than none. There is no default for this option. If it is not specified, RPC will not use security for the ping.`, ``),
        new Parameter(`/a <authn_level>`, `Specifies the authentication level to use. Possible values are:<br /><br />-   connect<br />-   call<br />-   pkt<br />-   integrity<br />-   privacy<br /><br />if this option is specified, the security package ID (/u) must also be specified. There is no default for this option.<br /><br />if this option is not specified, RPC will not use security for the ping.`, ``),
        new Parameter(`/N <server_princ_name>`, `Specifies a server principal name.<br /><br />This field can be used only when authentication level and security package are selected.`, ``),
        new Parameter(`/I <auth_identity>`, `Allows you to specify alternative identity to connect to the server. The identity is in the form user,domain,password. If the user name, domain, or password have special characters that can be interpreted by the shell, enclose the identity in double quotes. You can specify * instead of the password and RPC will prompt you to enter the password without echoing it on the screen. If this field is not specified, the identity of the logged on user will be used.<br /><br />This field can be used only when authentication level and security package are selected.`, ``),
        new Parameter(`/C <capabilities>`, `Specifies a hexadecimal bitmask of flags. This field can be used only when authentication level and security package are selected.`, ``),
        new Parameter(`/T <identity_tracking>`, `Specifies static or dynamic. If not specified, dynamic is the default.<br /><br />This field can be used only when authentication level and security package are selected.`, ``),
        new Parameter(`/M <impersonation_type>`, `Specifies anonymous, identify, impersonate or delegate. Default is impersonate.<br /><br />This field can be used only when authentication level and security package are selected.`, ``),
        new Parameter(`/S <server_sid>`, `Specifies the expected SID of the server.<br /><br />This field can be used only when authentication level and security package are selected.`, ``),
        new Parameter(`/P <proxy_auth_identity>`, `Specifies the identity to authenticate with to the RPC/HTTP proxy. Has the same format as for the /I option. You must specify security package (/u), authentication level (/a), and authentication schemes (/H) in order to use this option.`, ``),
        new Parameter(`/F <RPCHTTP_flags>`, `Specifies the flags to pass for RPC/HTTP front end authentication. The flags may be specified as numbers or names The currently recognized flags are:<br /><br />-   Use SSL / 1 or ssl or use_ssl<br />-   Use first auth scheme / 2 or first or use_first<br /><br />You must specify security package (/u) and authentication level (/a) in order to use this option.`, ``),
        new Parameter(`/H <RPC/HTTP_authn_schemes>`, `Specifies the authentication schemes to use for RPC/HTTP front end authentication. This option is a list of numerical values or names separated by comma. Example: Basic,NTLM. Recognized values are (names are not case sensitive):<br /><br />-   Basic / 1 or Basic<br />-   NTLM / 2 or NTLM<br />-   Certificate / 65536 or Cert<br /><br />You must specify security package (/u) and authentication level (/a) in order to use this option.`, ``),
        new Parameter(`/B <server_certificate_subject>`, `Specifies the server certificate subject. You must use SSL for this option to work.<br /><br />You must specify security package (/u) and authentication level (/a) in order to use this option.`, ``),
        new Parameter(`/b`, `Retrieves the server certificate subject from the certificate sent by the server and prints it to a screen or a log file. Valid only when the Proxy echo only option (/E) and the use SSL options are specified.<br /><br />You must specify security package (/u) and authentication level (/a) in order to use this option.`, ``),
        new Parameter(`/R`, `Specifies the HTTP proxy. If *none*, the RPC proxy is used. The value *default* means to use the IE settings in your client machine. Any other value will be treated as the explicit HTTP proxy. If you do not specify this flag, the default value is assumed, that is, the IE settings are checked. This flag is valid only when the /E (echo Only) flag is enabled.`, ``),
        new Parameter(`/E`, `Restricts the ping to the RPC/HTTP proxy only. The ping does not reach the server. Useful when trying to establish whether the RPC/HTTP proxy is reachable. To specify an HTTP proxy, use the /R flag. If an HTTP proxy is specified in the /o flag, this option will be ignored.<br /><br />You must specify security package (/u) and authentication level (/a) in order to use this option.`, ``),
        new Parameter(`/q`, `Specifies quiet mode. Does not issue any prompts except for passwords. Assumes *Y* response to all queries. Use this option with care.`, ``),
        new Parameter(`/c`, `Use smart card certificate. rpcping will prompt user to choose smart card.`, ``),
        new Parameter(`/A`, `Specifies the identity with which to authenticate to the HTTP proxy. Has the same format as for the /I option.<br /><br />You must specify authentication schemes (/U), security package (/u) and authentication level (/a) in order to use this option.`, ``),
        new Parameter(`/U`, `Specifies the authentication schemes to use for HTTP proxy authentication. This option is a list of numerical values or names separated by comma. Example: Basic,NTLM. Recognized values are (names are not case sensitive):<br /><br />-   Basic / 1 or Basic<br />-   NTLM / 2 or NTLM<br /><br />You must specify security package (/u) and authentication level (/a) in order to use this option.`, ``),
        new Parameter(`/r`, `if multiple iterations are specified, this option will make rpcping display current execution statistics periodically instead after the last call. The report interval is given in seconds. Default is 15.`, ``),
        new Parameter(`/v`, `Tells rpcping how verbose to make the output. Default value is 1. 2 and 3 provide more output from rpcping.`, ``),
        new Parameter(`/d`, `Launches RPC network diagnostic UI.`, ``),
        new Parameter(`/p`, `Specifies to prompt for credentials if authentication fails.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Confirms the RPC connectivity between the computer running Microsoft Exchange Server and any of the supported Microsoft Exchange Client workstations on the network. This utility can be used to check if the Microsoft Exchange Server services are responding to RPC requests from the client workstations via the network. `, `rpcping [/t <protseq>] [/s <server_addr>] [/e <endpoint>

        |/f <interface UUID>[,Majorver]] [/O <Interface Object UUID]

        [/i <#_iterations>] [/u <security_package_id>] [/a <authn_level>]

        [/N <server_princ_name>] [/I <auth_identity>] [/C <capabilities>]

        [/T <identity_tracking>] [/M <impersonation_type>]

        [/S <server_sid>] [/P <proxy_auth_identity>] [/F <RPCHTTP_flags>]

        [/H <RPC/HTTP_authn_schemes>] [/o <binding_options>]

        [/B <server_certificate_subject>] [/b] [/E] [/q] [/c]

        [/A <http_proxy_auth_identity>] [/U <HTTP_proxy_authn_schemes>]

        [/r <report_results_interval>] [/v <verbose_level>] [/d]`, "", () => { }),
    new ConsoleCommand(`rsh`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Runs commands on remote computers running the RSH service or daemon. This command has been deprecated. You can install the Subsystem for UNIX-based Applications using the Add Features Wizard. For more information, see [Windows Server 2008 UNIX Interoperability Components](https://go.microsoft.com/fwlink/?LinkId=191835) at the Microsoft Web site. After installation, you can then open a C Shell or Korn Shell command window and run rsh. For more information, type man rsh at the C Shell or Korn Shell prompt.`, ``, "", () => { }),
    new ConsoleCommand(`rundll32`, [
        new Parameter(`[Rundll32 printui.dll,PrintUIEntry](rundll32-printui.md)`, `Displays the printer user interface`, ``),
    ], `Loads and runs 32-bit dynamic-link libraries (DLLs). There are no configurable settings for Rundll32. Help information is provided for a specific DLL you run with the rundll32 command.`, `Rundll32 <DLLname>`, "", () => { }),
    new ConsoleCommand(`rwinsta`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `Enables you to reset (delete) a session on a Remote Desktop Session Host (rd Session Host) server.`, ``, "", () => { }),
    new ConsoleCommand(`schtasks`, [
        new Parameter(`MINUTE, HOURLY, DAILY, WEEKLY, MONTHLY`, `Specifies the time unit for the schedule.`, ``),
        new Parameter(`ONCE`, `The task runs once at a specified date and time.`, ``),
        new Parameter(`ONSTART`, `The task runs every time the system starts. You can specify a start date, or run the task the next time the system starts.`, ``),
        new Parameter(`ONLOGON`, `The task runs whenever a user (any user) logs on. You can specify a date, or run the task the next time the user logs on.`, ``),
        new Parameter(`ONIDLE`, `The task runs whenever the system is idle for a specified period of time. You can specify a date, or run the task the next time the system is idle.`, ``),
    ], `Schedules commands and programs to run periodically or at a specific time. Adds and removes tasks from the schedule, starts and stops tasks on demand, and displays and changes scheduled tasks.`, `ERROR: The data is invalid.`, "", () => { }),
    new ConsoleCommand(`Scwcmd`, [
        new Parameter(`/analyze`, `Determines whether a computer is in compliance with a policy.</br>See [Scwcmd: analyze](scwcmd-analyze.md) for syntax and options.`, ``),
        new Parameter(`/configure`, `Applies an SCW-generated security policy to a computer.</br>See [Scwcmd: configure](scwcmd-configure.md) for syntax and options.`, ``),
        new Parameter(`/register`, `Extends or customizes the SCW Security Configuration Database by registering a Security Configuration Database file that contains role, task, service, or port definitions.</br>See [Scwcmd: register](scwcmd-register.md) for syntax and options.`, ``),
        new Parameter(`/rollback`, `Applies the most recent rollback policy available, and then deletes that rollback policy.</br>See [Scwcmd: rollback](scwcmd-rollback.md) for syntax and options.`, ``),
        new Parameter(`/transform`, `Transforms a security policy file generated by using SCW into a new Group Policy object (GPO) in Active Directory Domain Services.</br>See [Scwcmd: transform](scwcmd-transform.md) syntax and options.`, ``),
        new Parameter(`/view`, `Renders an .xml file by using a specified .xsl transform.</br>See [Scwcmd: view](scwcmd-view.md) for syntax and options.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `The Scwcmd.exe command-line tool included with the Security Configuration Wizard (SCW) can be used to perform the following tasks:`, `scwcmd <command> [<subcommand>]`, "", () => { }),
    new ConsoleCommand(`secedit`, [
        new Parameter(`[Secedit:analyze](secedit-analyze.md)`, `Allows you to analyze current systems settings against baseline settings that are stored in a database.  The analysis results are stored in a separate area of the database and can be viewed in the Security Configuration and Analysis snap-in.`, ``),
        new Parameter(`[Secedit:configure](secedit-configure.md)`, `Allows you to configure a system with security settings stored in a database.`, ``),
        new Parameter(`[Secedit:export](secedit-export.md)`, `Allows you to export security settings stored in a database.`, ``),
        new Parameter(`[Secedit:generaterollback](secedit-generaterollback.md)`, `Allows you to generate a rollback template with respect to a configuration template.`, ``),
        new Parameter(`[Secedit:import](secedit-import.md)`, `Allows you to import a security template into a database so that the settings specified in the template can be applied to a system or analyzed against a system.`, ``),
        new Parameter(`[Secedit:validate](secedit-validate.md)`, `Allows you to validate the syntax of a security template.`, ``),
    ], `Configures and analyzes system security by comparing your current configuration to specified security templates.`, `secedit 

[/analyze /db <database file name> /cfg <configuration file name> [/overwrite] /log <log file name> [/quiet]]

[/configure /db <database file name> [/cfg <configuration filename>] [/overwrite] [/areas [securitypolicy | group_mgmt | user_rights | regkeys | filestore | services]] [/log <log file name>] [/quiet]]

[/export /db <database file name> [/mergedpolicy] /cfg <configuration file name> [/areas [securitypolicy | group_mgmt | user_rights | regkeys | filestore | services]] [/log <log file name>]]

[/generaterollback /db <database file name> /cfg <configuration file name> /rbk <rollback file name> [/log <log file name>] [/quiet]]

[/import /db <database file name> /cfg <configuration file name> [/overwrite] [/areas [securitypolicy | group_mgmt | user_rights | regkeys | filestore | services]] [/log <log file name>] [/quiet]]

[/validate <configuration file name>]`, "", () => { }),
    new ConsoleCommand(`serverceipoptin`, [
        new Parameter(`/query`, `verifies the current setting.`, ``),
        new Parameter(`/enable`, `Enables participation.`, ``),
        new Parameter(`/disable`, `Disables participation.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Allows you to participate in the Customer Experience Improvement Program (CEIP).`, `serverceipoptin [/query] [/enable] [/disable]`, "", () => { }),
    new ConsoleCommand(`Servermanagercmd`, [
        new Parameter(`-query [[[<Drive>:]<path>]<*query.xml*>]`, `Displays a list of all roles, role services, and features installed and available for installation on the server. You can also use the short form of this parameter, -q. If you want the query results saved to an XML file, specify an XML file to replace *query.xml*.`, ``),
        new Parameter(`-inputpath  <[[<Drive>:]<path>]*answer.xml*>`, `Installs or removes the roles, role services, and features specified in an XML answer file represented by *answer.xml*. You can also use the short form of this parameter, -p.`, ``),
        new Parameter(`-install <*Id*>`, `Installs the role, role service, or feature specified by *Id*. The identifiers are case-insensitive. Multiple roles, role services, and features must be separated by spaces. The following optional parameters are used with the -install parameter.<br /><br />-   -setting <*SettingName*>=<*SettingValue*>   Specifies required settings for the installation.<br />-   -allSubFeatures Specifies the installation of all subordinate services and features along with the parent role, role service, or feature named in the *Id* value. Note:     Some role containers do not have a command line identifier to allow installation of all role services. This is the case when role services cannot be installed in the same instance of the Server Manager command. For example, the Federation Service role service of active directory Federation Services and the Federation Service Proxy role service cannot be installed by using the same Server Manager command instance.<br />-   -resultpath <*result.xml>   Saves installation results to an XML file represented by *result.xml*. You can also use the short form of this parameter, -r. Note:     You cannot run servermanagercmd with both the -resultpath parameter and the -whatif parameter specified.<br />-   -restart Restarts the computer automatically when installation is complete (if restarting is required by the roles or features installed).<br />-   -whatif Displays any operations specified for the -install parameter. You can also use the short form of the -whatif parameter, -w. You cannot run servermanagercmd with both the -resultpath parameter and the -whatif parameter specified.<br />-   -logpath <[[<Drive>:]<path>]*log.txt*>   Specifies a name and location for the log file, other than the default, %windir%tempservermanager.log.`, ``),
        new Parameter(`-remove <*Id*>`, `Removes the role, role service, or feature specified by *Id*. The identifiers are case-insensitive. Multiple roles, role services, and features must be separated by spaces. The following optional parameters are used with the -remove parameter.<br /><br />-   -resultpath <[[<Drive>:]<path>]*result.xml*>   Saves removal results to an XML file represented by *result.xml*. You can also use the short form of this parameter, -r. Note:     You cannot run servermanagercmd with both the -resultpath parameter and the -whatif parameter specified.<br />-   -restart Restarts the computer automatically when removal is complete (if restarting is required by remaining roles or features).<br />-   -whatif Displays any operations specified for the -remove parameter. You can also use the short form of the -whatif parameter, -w. You cannot run servermanagercmd with both the -resultpath parameter and the -whatif parameter specified.<br />-   -logpath<[[<Drive>:]<path>]*log.txt*>   Specifies a name and location for the log file, other than the default, %windir%tempservermanager.log.`, ``),
        new Parameter(`-help`, `Displays help in the Command prompt window. You can also use the short form, -?.`, ``),
        new Parameter(`-version`, `Displays the Server Manager version number. You can also use the short form, -v.`, ``),
    ], `Installs and removes roles, role services, and features. Also displays the list of all roles, role services, and features available, and shows which are installed on this computer. For additional information about the roles, role services, and features that you can specify by using this tool, see the [Server Manager help](https://go.microsoft.com/fwlink/?LinkID=137387). For examples of how to use this command, see [Examples](#BKMK_examples).`, `servermanagercmd -query [[[<Drive>:]<path>]<query.xml>] [-logpath   [[<Drive>:]<path>]<log.txt>]

servermanagercmd -inputpath  [[<Drive>:]<path>]<answer.xml> [-resultpath <result.xml> [-restart] | -whatif] [-logpath [[<Drive>:]<path>]<log.txt>]

servermanagercmd -install <Id> [-allSubFeatures] [-resultpath   [[<Drive>:]<path>]<result.xml> [-restart] | -whatif] [-logpath   [[<Drive>:]<path>]<log.txt>]

servermanagercmd -remove <Id> [-resultpath    <result.xml> [-restart] | -whatif] [-logpath  [[<Drive>:]<path>]<log.txt>]

servermanagercmd [-help | -?]

servermanagercmd -version`, "", () => { }),
    new ConsoleCommand(`set`, [
        new Parameter(`<Variable>`, `Specifies the environment variable to set or modify.`, ``),
        new Parameter(`<String>`, `Specifies the string to associate with the specified environment variable.`, ``),
        new Parameter(`/p`, `Sets the value of *Variable* to a line of input entered by the user.`, ``),
        new Parameter(`<PromptString>`, `Optional. Specifies a message to prompt the user for input. This parameter is used with the /p command-line option.`, ``),
        new Parameter(`/a`, `Sets *String* to a numerical expression that is evaluated.`, ``),
        new Parameter(`<Expression>`, `Specifies a numerical expression. See Remarks for valid operators that can be used in *Expression*.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays, sets, or removes CMD.EXE environment variables. If used without parameters, set displays the current environment variable settings.`, `set [<Variable>=[<String>]]

set [/p] <Variable>=[<PromptString>]

set /a <Variable>=<Expression>`, "", () => { }),
    new ConsoleCommand(`setlocal`, [
        new Parameter(`enableextensions`, `Enables the command extensions until the matching endlocal command is encountered, regardless of the setting before the setlocal command was run.`, ``),
        new Parameter(`disableextensions`, `Disables the command extensions until the matching endlocal command is encountered, regardless of the setting before the setlocal command was run.`, ``),
        new Parameter(`enabledelayedexpansion`, `Enables the delayed environment variable expansion until the matching endlocal command is encountered, regardless of the setting before the setlocal command was run.`, ``),
        new Parameter(`disabledelayedexpansion`, `Disables the delayed environment variable expansion until the matching endlocal command is encountered, regardless of the setting before the setlocal command was run.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Starts localization of environment variables in a batch file. Localization continues until a matching endlocal command is encountered or the end of the batch file is reached.`, `setlocal [enableextensions | disableextensions] [enabledelayedexpansion | disabledelayedexpansion]`, "", () => { }),
    new ConsoleCommand(`setx`, [
        new Parameter(`/s <Computer>`, `Specifies the name or IP address of a remote computer. Do not use backslashes. The default value is the name of the local computer.`, ``),
        new Parameter(`/u [<Domain>]<User name>`, `Runs the script with the credentials of the specified user account. The default value is the system permissions.`, ``),
        new Parameter(`/p [<Password>]`, `Specifies the password of the user account that is specified in the /u parameter.`, ``),
        new Parameter(`<Variable>`, `Specifies the name of the environment variable that you want to set.`, ``),
        new Parameter(`<Value>`, `Specifies the value to which you want to set the environment variable.`, ``),
        new Parameter(`/k <Path>`, `Specifies that the variable is set based on information from a registry key. The p*ath* uses the following syntax:</br>"\<HIVE><KEY>...<Value>"</br>For example, you might specify the following path:</br>"HKEY_LOCAL_MACHINESystemCurrentControlSetControlTimeZoneInformationStandardName"`, ``),
        new Parameter(`/f <File name>`, `Specifies the file that you want to use.`, ``),
        new Parameter(`/a <X>,<Y>`, `Specifies absolute coordinates and offset as search parameters.`, ``),
        new Parameter(`/r <X>,<Y> "<String>"`, `Specifies relative coordinates and offset from String as search parameters.`, ``),
        new Parameter(`/m`, `Specifies to set the variable in the system environment. The default setting is the local environment.`, ``),
        new Parameter(`/x`, `Displays file coordinates, ignoring the /a, /r, and /d command-line options.`, ``),
        new Parameter(`/d <Delimiters>`, `Specifies delimiters such as "," or "" to be used in addition to the four built-in delimiters â€” SPACE, TAB, ENTER, and LINEFEED. Valid delimiters include any ASCII character. The maximum number of delimiters is 15, including built-in delimiters.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Creates or modifies environment variables in the user or system environment, without requiring programming or scripting. The Setx command also retrieves the values of registry keys and writes them to text files.`, `setx [/s <Computer> [/u [<Domain>\]<User name> [/p [<Password>]]]] <Variable> <Value> [/m]

setx [/s <Computer> [/u [<Domain>\]<User name> [/p [<Password>]]]] [<Variable>] /k <Path> [/m]

setx [/s <Computer> [/u [<Domain>\]<User name> [/p [<Password>]]]] /f <FileName> {[<Variable>] {/a <X>,<Y> | /r <X>,<Y> "<String>"} [/m] | /x} [/d <Delimiters>]`, "", () => { }),
    new ConsoleCommand(`sfc`, [
        new Parameter(`/scannow`, `Scans the integrity of all protected system files and repairs files with problems when possible.`, ``),
        new Parameter(`/verifyonly`, `Scans integrity of all protected system files. No repair operation is performed.`, ``),
        new Parameter(`/scanfile`, `Scans integrity of the specified file and repairs the file if problems are detected, when possible.`, ``),
        new Parameter(`<file>`, `Specified full path and filename`, ``),
        new Parameter(`/verifyfile`, `verifies the integrity of the specified file. No repair operation is performed.`, ``),
        new Parameter(`/offwindir`, `Specifies the location of the offline windows directory, for offline repair.`, ``),
        new Parameter(`/offbootdir`, `Specifies the location of the offline boot directory for offline`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Scans and verifies the integrity of all protected system files and replaces incorrect versions with correct versions.`, `sfc [/scannow] [/verifyonly] [/scanfile=<file>] [/verifyfile=<file>] [/offwindir=<offline windows directory> /offbootdir=<offline boot directory>]`, "", () => { }),
    new ConsoleCommand(`shadow`, [
        new Parameter(`<SessionName>`, `Specifies the name of the session that you want to remotely control.`, ``),
        new Parameter(`<SessionID>`, `Specifies the ID of the session that you want to remotely control. Use query user to display the list of sessions and their session IDs.`, ``),
        new Parameter(`/server:<ServerName>`, `Specifies the rd Session Host server containing the session that you want to remotely control. By default, the current rd Session Host4 server is used.`, ``),
        new Parameter(`/v`, `Displays information about the actions being performed.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Enables you to remotely control an active session of another user on a Remote Desktop Session Host (rd Session Host) server.`, `shadow {<SessionName> | <SessionID>} [/server:<ServerName>] [/v]`, "", () => { }),
    new ConsoleCommand(`shift`, [
        new Parameter(`/n <N>`, `Specifies to start shifting at the *N*th argument, where *N* is any value from 0 to 8. Requires command extensions, which are enabled by default.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Changes the position of batch parameters in a batch file.`, `shift [/n <N>]`, "", () => { }),
    new ConsoleCommand(`showmount`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
        new Parameter(`#`, ``, ``),
    ], `You can use showmount to display mounted directories.  `, `showmount {-e|-a|-d} <Server>`, "", () => { }),
    new ConsoleCommand(`shutdown`, [
        new Parameter(`/i`, `Displays the Remote Shutdown Dialog box. The /i option must be the first parameter following the command. If /i is specified, all other options are ignored.`, ``),
        new Parameter(`/l`, `Logs off the current user immediately, with no time-out period. You cannot use /l with /m or /t.`, ``),
        new Parameter(`/s`, `Shuts down the computer.`, ``),
        new Parameter(`/r`, `Restarts the computer after shutdown.`, ``),
        new Parameter(`/a`, `Aborts a system shutdown. Effective only during the timeout period. To use /a, you must also use the /m option.`, ``),
        new Parameter(`/p`, `Turns off the local computer only (not a remote computer)â€”with no time-out period or warning. You can use /p only with /d or /f. If your computer does not support power-off functionality, it will shut down when you use /p, but the power to the computer will remain on.`, ``),
        new Parameter(`/h`, `Puts the local computer into hibernation, if hibernation is enabled. You can use /h only with /f.`, ``),
        new Parameter(`/e`, `Enables you to document the reason for the unexpected shutdown on the target computer.`, ``),
        new Parameter(`/f`, `Forces running applications to close without warning users.</br>Caution: Using the /f option might result in loss of unsaved data.`, ``),
        new Parameter(`/m \\<ComputerName>`, `Specifies the target computer. Cannot be used with the /l option.`, ``),
        new Parameter(`/t <XXX>`, `Sets the time-out period or delay to *XXX* seconds before a restart or shutdown. This causes a warning to display on the local console. You can specify 0-600 seconds. If you do not use /t, the time-out period is 30 seconds by default.`, ``),
        new Parameter(`/d [p|u:]<XX>:<YY>`, `Lists the reason for the system restart or shutdown. The following are the parameter values:</br>p Indicates that the restart or shutdown is planned.</br>u Indicates that the reason is user defined.</br>Note: If p or u are not specified, the restart or shutdown is unplanned.</br>*XX* Specifies the major reason number (positive integer less than 256).</br>*YY* Specifies the minor reason number (positive integer less than 65536).`, ``),
        new Parameter(`/c "<Comment>"`, `Enables you to comment in detail about the reason for the shutdown. You must first provide a reason by using the /d option. You must enclose comments in quotation marks. You can use a maximum of 511 characters.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt, including a list of the major and minor reasons that are defined on your local computer.`, ``),
    ], `Enables you to shut down or restart local or remote computers one at a time.`, `shutdown [/i | /l | /s | /r | /a | /p | /h | /e] [/f] [/m \\<ComputerName>] [/t <XXX>] [/d [p|u:]<XX>:<YY> [/c "comment"]]`, "", () => { }),
    new ConsoleCommand(`sort`, [
        new Parameter(`/r`, `Reverses the sort order (that is, sorts from Z to A and from 9 to 0).`, ``),
        new Parameter(`/+<N>`, `Specifies the character position number where sort will begin each comparison. *N* can be any valid integer.`, ``),
        new Parameter(`/m <Kilobytes>`, `Specifies the amount of main memory to use for the sort in kilobytes (KB).`, ``),
        new Parameter(`/l <Locale>`, `Overrides the sort order of characters that are defined by the system default locale (that is, the language and Country/Region selected during installation).`, ``),
        new Parameter(`/rec <Characters>`, `Specifies the maximum number of characters in a record or a line of the input file (the default value is 4,096 and the maximum is 65,535).`, ``),
        new Parameter(`[<Drive1>:][<Path1>]<FileName1>`, `Specifies the file to be sorted. If no file name is specified, the standard input is sorted. Specifying the input file is faster than redirecting the same file as standard input.`, ``),
        new Parameter(`/t [<Drive2>:][<Path2>]`, `Specifies the path of the directory to hold the sort command's working storage if the data does not fit in the main memory. By default, the system temporary directory is used.`, ``),
        new Parameter(`/o [<Drive3>:][<Path3>]<FileName3>`, `Specifies the file where the sorted input is to be stored. If not specified, the data is written to the standard output. Specifying the output file is faster than redirecting standard output to the same file.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Reads input, sorts data, and writes the results to the screen, to a file, or to another device.`, `sort [/r] [/+<N>] [/m <Kilobytes>] [/l <Locale>] [/rec <Characters>] [[<Drive1>:][<Path1>]<FileName1>] [/t [<Drive2>:][<Path2>]] [/o [<Drive3>:][<Path3>]<FileName3>]`, "", () => { }),
    new ConsoleCommand(`start`, [
        new Parameter(`"<Title>"`, `Specifies the title to display in the Command Prompt window title bar.`, ``),
        new Parameter(`/d <Path>`, `Specifies the startup directory.`, ``),
        new Parameter(`/i`, `Passes the Cmd.exe startup environment to the new Command Prompt window. If /i is not specified, the current environment is used.`, ``),
        new Parameter(`/min | /max`, `Specifies to minimize (/min) or maximize (/max) the new Command Prompt window.`, ``),
        new Parameter(`/separate | /shared`, `Starts 16-bit programs in a separate memory space (/separate) or shared memory space (/shared). These options are not supported on 64-bit platforms.`, ``),
        new Parameter(`/low | /normal | /high | /realtime | /abovenormal | /belownormal`, `Starts an application in the specified priority class. Valid priority class values are /low, /normal, /high, /realtime, /abovenormal, and /belownormal.`, ``),
        new Parameter(`/affinity <HexAffinity>`, `Applies the specified processor affinity mask (expressed as a hexadecimal number) to the new application.`, ``),
        new Parameter(`/wait`, `Starts an application and waits for it to end.`, ``),
        new Parameter(`/b`, `Starts an application without opening a new Command Prompt window. CTRL+C handling is ignored unless the application enables CTRL+C processing. Use CTRL+BREAK to interrupt the application.`, ``),
        new Parameter(`/b <Command> | <Program>`, `Specifies the command or program to start.`, ``),
        new Parameter(`<Parameters>`, `Specifies parameters to pass to the command or program.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Starts a separate Command Prompt window to run a specified program or command.`, `start ["<Title>"] [/d <Path>] [/i] [{/min | /max}] [{/separate | /shared}] [{/low | /normal | /high | /realtime | /abovenormal | belownormal}] [/affinity <HexAffinity>] [/wait] [/b {<Command> | <Program>} [<Parameters>]]`, "", () => { }),
    new ConsoleCommand(`subst`, [
        new Parameter(`<Drive1>:`, `Specifies the virtual drive to which you want to assign a path.`, ``),
        new Parameter(`[<Drive2>:]<Path>`, `Specifies the physical drive and path that you want to assign to a virtual drive.`, ``),
        new Parameter(`/d`, `Deletes a substituted (virtual) drive.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Associates a path with a drive letter. If used without parameters, subst displays the names of the virtual drives in effect.`, `subst [<Drive1>: [<Drive2>:]<Path>] 

subst <Drive1>: /d`, "", () => { }),
    new ConsoleCommand(`sxstrace`, [
        new Parameter(`trace`, `Enables tracing for sxs (side-by-side)`, ``),
        new Parameter(`/logfile`, `Specifies the raw log file.`, ``),
        new Parameter(`<FileName>`, `Saves tracing log to *FileName*.`, ``),
        new Parameter(`/nostop`, `Specifies no prompt to stop tracing.`, ``),
        new Parameter(`parse`, `Translates the raw trace file.`, ``),
        new Parameter(`/outfile`, `Specifies the output filename.`, ``),
        new Parameter(`<ParsedFile>`, `Specifies the filename of the parsed file.`, ``),
        new Parameter(`/filter`, `Allows the output to be filtered.`, ``),
        new Parameter(`<AppName>`, `Specifies the name of the application.`, ``),
        new Parameter(`stoptrace`, `Stop the trace if it is not stopped before.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Diagnoses side-by-side problems.    `, `sxstrace [{[trace /logfile:<FileName> [/nostop]|[parse /logfile:<FileName> /outfile:<ParsedFile>  [/filter:<AppName>]}]`, "", () => { }),
    new ConsoleCommand(`sysocmgr`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Sysocmgr is deprecated, and is not guaranteed to be supported in future releases of Windows.`, ``, "", () => { }),
    new ConsoleCommand(`systeminfo`, [
        new Parameter(`/s <Computer>`, `Specifies the name or IP address of a remote computer (do not use backslashes). The default is the local computer.`, ``),
        new Parameter(`/u <Domain><UserName>`, `Runs the command with the account permissions of the specified user account. If /u is not specified, this command uses the permissions of the user who is currently logged on to the computer that is issuing the command.`, ``),
        new Parameter(`/p <Password>`, `Specifies the password of the user account that is specified in the /u parameter.`, ``),
        new Parameter(`/fo <Format>`, `Specifies the output format with one of the following values:</br>TABLE: Displays output in a table.</br>LIST: Displays output in a list.</br>CSV: Displays output in Comma Separated Values format.`, ``),
        new Parameter(`/nh`, `Suppresses column headers in the output. Valid when the /fo parameter is set to TABLE or CSV.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays detailed configuration information about a computer and its operating system, including operating system configuration, security information, product ID, and hardware properties (such as RAM, disk space, and network cards).`, `Systeminfo [/s <Computer> [/u <Domain>\<UserName> [/p <Password>]]] [/fo {TABLE | LIST | CSV}] [/nh]`, "", () => { }),
    new ConsoleCommand(`takeown`, [
        new Parameter(`/s <Computer>`, `Specifies the name or IP address of a remote computer (do not use backslashes). The default value is the local computer. This parameter applies to all of the files and folders specified in the command.`, ``),
        new Parameter(`/u [<Domain>]<User name>`, `Runs the script with the permissions of the specified user account. The default value is system permissions.`, ``),
        new Parameter(`/p [<Password>]`, `Specifies the password of the user account that is specified in the /u parameter.`, ``),
        new Parameter(`/f <File name>`, `Specifies the file name or directory name pattern. You can use the wildcard character * when specifying the pattern. You can also use the syntax *ShareNameFileName*.`, ``),
        new Parameter(`/a`, `Gives ownership to the Administrators group instead of the current user.`, ``),
        new Parameter(`/r`, `Performs a recursive operation on all files in the specified directory and subdirectories.`, ``),
        new Parameter(`/d {Y | N}`, `Suppresses the confirmation prompt that is displayed when the current user does not have the "List Folder" permission on a specified directory, and instead uses the specified default value. Valid values for the /d option are as follows:</br>-   Y: Take ownership of the directory.</br>-   N: Skip the directory.</br>Note that you must use this option in conjunction with the /r option.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Enables an administrator to recover access to a file that previously was denied, by making the administrator the owner of the file.`, `takeown [/s <Computer> [/u [<Domain>\]<User name> [/p [<Password>]]]] /f <File name> [/a] [/r [/d {Y|N}]]`, "", () => { }),
    new ConsoleCommand(`tapicfg`, [
        new Parameter(`install /directory:<PartitionName>`, `Required. Specifies the DNS name of the TAPI application directory partition to be created. This name must be a fully qualified domain name.`, ``),
        new Parameter(`/server: <DCName>`, `Specifies the DNS name of the domain controller on which the TAPI application directory partition is created. If the domain controller name is not specified, the name of the local computer is used.`, ``),
        new Parameter(`/forcedefault`, `Specifies that this directory is the default TAPI application directory partition for the domain. There can be multiple TAPI application directory partitions in a domain.<br /><br />if this directory is the first TAPI application directory partition created on the domain, it is automatically set as the default, regardless of whether you use the /forcedefault option.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Creates, removes, or displays a TAPI application directory partition, or sets a default TAPI application directory partition. TAPI 3.1 clients can use the information in this application directory partition with the directory service locator service to find and communicate with TAPI directories.You can also use tapicfg to create or remove service connection points, which enable TAPI clients to efficiently locate TAPI application directory partitions in a domain. For more information, see remarks. To view the command syntax, click a command. `, `tapicfg install /directory:<PartitionName> [/server:<DCName>] [/forcedefault]`, "", () => { }),
    new ConsoleCommand(`taskkill`, [
        new Parameter(`/s <computer>`, `Specifies the name or IP address of a remote computer (do not use backslashes). The default is the local computer.`, ``),
        new Parameter(`/u <Domain>\\<UserName>`, `Runs the command with the account permissions of the user who is specified by *UserName* or *Domain*\*UserName*. /u can be specified only if /s is specified. The default is the permissions of the user who is currently logged on to the computer that is issuing the command.`, ``),
        new Parameter(`/p <Password>`, `Specifies the password of the user account that is specified in the /u parameter.`, ``),
        new Parameter(`/fi <Filter>`, `Applies a filter to select a set of tasks. You can use more than one filter or use the wildcard character (*) to specify all tasks or image names. See the following [table for valid filter names](#BKMK_table), operators, and values.`, ``),
        new Parameter(`/pid <ProcessID>`, `Specifies the process ID of the process to be terminated.`, ``),
        new Parameter(`/im <ImageName>`, `Specifies the image name of the process to be terminated. Use the wildcard character (*) to specify all image names.`, ``),
        new Parameter(`/f`, `Specifies that processes be forcefully terminated. This parameter is ignored for remote processes; all remote processes are forcefully terminated.`, ``),
        new Parameter(`/t`, `Terminates the specified process and any child processes started by it.`, ``),
    ], `Ends one or more tasks or processes. Processes can be ended by process ID or image name. taskkill replaces the kill tool.`, `taskkill [/s <computer> [/u [<Domain>\]<UserName> [/p [<Password>]]]] {[/fi <Filter>] [...] [/pid <ProcessID> | /im <ImageName>]} [/f] [/t]`, "", () => { }),
    new ConsoleCommand(`tasklist`, [
        new Parameter(`/s <Computer>`, `Specifies the name or IP address of a remote computer (do not use backslashes). The default is the local computer.`, ``),
        new Parameter(`/u [<Domain>\\]<UserName>`, `Runs the command with the account permissions of the user who is specified by *UserName* or *DomainUserName*. /u can be specified only if /s is specified. The default is the permissions of the user who is currently logged on to the computer that is issuing the command.`, ``),
        new Parameter(`/p <Password>`, `Specifies the password of the user account that is specified in the /u parameter.`, ``),
        new Parameter(`/m <Module>`, `Lists all tasks with DLL modules loaded that match the given pattern name. If the module name is not specified, this option displays all modules loaded by each task.`, ``),
        new Parameter(`/svc`, `Lists all the service information for each process without truncation. Valid when the /fo parameter is set to table.`, ``),
        new Parameter(`/v`, `Displays verbose task information in the output. For complete verbose output without truncation, use /v and /svc together.`, ``),
        new Parameter(`/fo {table | list | csv}`, `Specifies the format to use for the output. Valid values are table, list, and csv. The default format for output is table.`, ``),
        new Parameter(`/nh`, `Suppresses column headers in the output. Valid when the /fo parameter is set to table or csv.`, ``),
        new Parameter(`/fi <Filter>`, `Specifies the types of processes to include in or exclude from the query. See the following table for valid filter names, operators, and values.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays a list of currently running processes on the local computer or on a remote computer. Tasklist replaces the tlist tool.`, `tasklist [/s <Computer> [/u [<Domain>\]<UserName> [/p <Password>]]] [{/m <Module> | /svc | /v}] [/fo {table | list | csv}] [/nh] [/fi <Filter> [/fi <Filter> [ ... ]]]`, "", () => { }),
    new ConsoleCommand(`tcmsetup`, [
        new Parameter(`/q`, `Prevents the display of message boxes.`, ``),
        new Parameter(`/x`, `Specifies that connection-oriented callbacks will be used for heavy traffic networks where packet loss is high. When this parameter is omitted, connectionless callbacks will be used.`, ``),
        new Parameter(`/c`, `Required. Specifies client setup.`, ``),
        new Parameter(`<Server1>`, `Required. Specifies the name of the remote server that has the TAPI service providers that the client will use. The client will use the service providers' lines and phones. The client must be in the same domain as the server or in a domain that has a two-way trust relationship with the domain that contains the server.`, ``),
        new Parameter(`<Server2>â€¦`, `Specifies any additional server or servers that will be available to this client. If you specify a list of servers is, use a space to separate the server names.`, ``),
        new Parameter(`/d`, `Clears the list of remote servers. Disables the TAPI client by preventing it from using the TAPI service providers that are on the remote servers.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Sets up or disables the TAPI client.`, `tcmsetup [/q] [/x] /c <Server1> [<Server2> â€¦] 

tcmsetup  [/q] /c /d`, "", () => { }),
    new ConsoleCommand(`telnet`, [
        new Parameter(`/a`, `attempt automatic logon. Same as /l option except uses the currently logged on user s name.`, ``),
        new Parameter(`/e <EscapeChar>`, `Escape character used to enter the telnet client prompt.`, ``),
        new Parameter(`/f <FileName>`, `File name used for client side logging.`, ``),
        new Parameter(`/l <UserName>`, `Specifies the user name to log on with on the remote computer.`, ``),
        new Parameter(`/t {vt100 &#124; vt52 &#124; ansi &#124; vtnt}`, `Specifies the terminal type. Supported terminal types are vt100, vt52, ansi, and vtnt.`, ``),
        new Parameter(`<Host> [<Port>]`, `Specifies the hostname or IP address of the remote computer to connect to, and optionally the TCP port to use (default is TCP port 23).`, ``),
        new Parameter(`/?`, `Displays help at the command prompt. Alternatively, you can type /h.`, ``),
    ], `Communicates with a computer running the telnet Server service. `, `telnet [/a] [/e <EscapeChar>] [/f <FileName>] [/l <UserName>] [/t {vt100 | vt52 | ansi | vtnt}] [<Host> [<Port>]] [/?]`, "", () => { }),
    new ConsoleCommand(`tftp`, [
        new Parameter(`-i`, `Specifies binary image transfer mode (also called octet mode). In binary image mode, the file is transferred in one-byte units. Use this mode when transferring binary files. If -i is omitted, the file is transferred in ASCII mode. This is the default transfer mode. This mode converts the end-of-line (EOL) characters to an appropriate format for the specified computer. Use this mode when transferring text files. If a file transfer is successful, the data transfer rate is displayed.`, ``),
        new Parameter(`<Host>`, `Specifies the local or remote computer.`, ``),
        new Parameter(`put`, `Transfers the file *Source* on the local computer to the file *Destination* on the remote computer. Because the tftp protocol does not support user authentication, the user must be logged onto the remote computer, and the files must be writable on the remote computer.`, ``),
        new Parameter(`get`, `Transfers the file *Destination* on the remote computer to the file *Source* on the local computer.`, ``),
        new Parameter(`<Source>`, `Specifies the file to transfer.`, ``),
        new Parameter(`<Destination>`, `Specifies where to transfer the file.`, ``),
    ], `Transfers files to and from a remote computer, typically a computer running UNIX, that is running the Trivial File Transfer Protocol (tftp) service or daemon. tftp is typically used by embedded devices or systems that retrieve firmware, configuration information, or a system image during the boot process from a tftp server.   `, `tftp [-i] [<Host>] [{get | put}] <Source> [<Destination>]`, "", () => { }),
    new ConsoleCommand(`time`, [
        new Parameter(`<HH>[:<MM>[:<SS>[.<NN>]]] [am|pm]`, `Sets the system time to the new time specified, where *HH* is in hours (required), *MM* is in minutes, and *SS* is in seconds. *NN* can be used to specify hundredths of a second. If am or pm is not specified, time uses the 24-hour format by default.`, ``),
        new Parameter(`/t`, `Displays the current time without prompting you for a new time.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays or sets the system time. If used without parameters, time displays the current system time and prompts you to enter a new time.`, `time [/t | [<HH>[:<MM>[:<SS>]] [am|pm]]]`, "", () => { }),
    new ConsoleCommand(`timeout`, [
        new Parameter(`/t <TimeoutInSeconds>`, `Specifies the decimal number of seconds (between -1 and 99999) to wait before the command processor continues processing. The value -1 causes the computer to wait indefinitely for a keystroke.`, ``),
        new Parameter(`/nobreak`, `Specifies to ignore user key strokes.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Pauses the command processor for the specified number of seconds.`, `timeout /t <TimeoutInSeconds> [/nobreak]`, "", () => { }),
    new ConsoleCommand(`title`, [
        new Parameter(`<String>`, `Specifies the title of the Command Prompt window.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Creates a title for the Command Prompt window.`, `title [<String>]`, "", () => { }),
    new ConsoleCommand(`tlntadmn`, [
        new Parameter(`<computerName>`, `Specifies the name of the server to connect to. The default is the local computer.`, ``),
        new Parameter(`-u <UserName> -p <Password>`, `Specifies administrative credentials for a remote server that you want to administer. This parameter is required if you want to administer a remote server to which you are not logged on with administrative credentials.`, ``),
        new Parameter(`start`, `starts the telnet Server Service.`, ``),
        new Parameter(`stop`, `Stops the telnet Server Service`, ``),
        new Parameter(`pause`, `pauses the telnet Server Service. No new connections will be accepted.`, ``),
        new Parameter(`continue`, `Resumes the telnet Server Service.`, ``),
        new Parameter(`-s {<SessionID> &#124; all}`, `Displays active telnet sessions.`, ``),
        new Parameter(`-k {<SessionID> &#124; all}`, `Ends telnet sessions. type the Session ID to end a specific session, or type all to end all the sessions.`, ``),
        new Parameter(`-m {<SessionID> &#124; all}  <Message>`, `Sends a message to one or more sessions. type the session ID to send a message to a specific session, or type all to send a message to all sessions. type the message that you want to send between quotation marks.`, ``),
        new Parameter(`config dom = <Domain>`, `Configures the default domain for the server.`, ``),
        new Parameter(`config ctrlakeymap = {yes &#124; no}`, `Specifies if you want the telnet server to interpret CTRL+A as ALT. type yes to map the shortcut key, or type no to prevent the mapping.`, ``),
        new Parameter(`config timeout = <hh>:<mm>:<ss>`, `Sets the time-out period in hours, minutes, and seconds.`, ``),
        new Parameter(`config timeoutactive = {yes &#124; no`, `Enables the idle session timeout.`, ``),
        new Parameter(`config maxfail = <attempts>`, `Sets the maximum number of failed logon attempts before disconnecting.`, ``),
        new Parameter(`config maxconn = <Connections>`, `Sets the maximum number of connections.`, ``),
        new Parameter(`config port = <Number>`, `Sets the telnet port. You must specify the port with an integer smaller than 1024.`, ``),
        new Parameter(`config sec {+ &#124; -}NTLM {+ &#124; -}passwd`, `Specifies whether you want to use NTLM, a password, or both to authenticate logon attempts. To use a particular type of authentication, type a plus sign (+) before that type of authentication. To prevent using a particular type of authentication, type a minus sign (-) before that type of authentication.`, ``),
        new Parameter(`config mode = {console &#124; stream}`, `Specifies the mode of operation.`, ``),
        new Parameter(`-?`, `Displays help at the command prompt.`, ``),
    ], `Administers a local or remote computer that is running the telnet Server Service.   `, `tlntadmn [<computerName>] [-u <UserName>] [-p <Password>] [{start | stop | pause | continue}] [-s {<SessionID> | all}] [-k {<SessionID> | all}] [-m {<SessionID> | all}  <Message>] [config [dom = <Domain>] [ctrlakeymap = {yes | no}] [timeout = <hh>:<mm>:<ss>] [timeoutactive = {yes | no}] [maxfail = <attempts>] [maxconn = <Connections>] [port = <Number>] [sec {+ | -}NTLM {+ | -}passwd] [mode = {console | stream}]] [-?]`, "", () => { }),
    new ConsoleCommand(`tpmvscmgr`, [
        new Parameter(`/name`, `Required. Indicates the name of the new virtual smart card.`, ``),
        new Parameter(`/AdminKey`, `Indicates the desired administrator key that can be used to reset the PIN of the card if the user forgets the PIN.</br>DEFAULT Specifies the default value of 010203040506070801020304050607080102030405060708.</br>PROMPT Prompts the user to enter a value for the administrator key.</br>RANDOM Results in a random setting for the administrator key for a card that is not returned to the user. This creates a card that might not be manageable by using smart card management tools. When generated with RANDOM, the administrator key must be entered as 48 hexadecimal characters.`, ``),
        new Parameter(`/PIN`, `Indicates desired user PIN value.</br>DEFAULT Specifies the default PIN of 12345678.</br>PROMPT Prompts the user to enter a PIN at the command line. The PIN must be a minimum of eight characters, and it can contain numerals, characters, and special characters.`, ``),
        new Parameter(`/PUK`, `Indicates the desired PIN Unlock Key (PUK) value. The PUK value must be a minimum of eight characters, and it can contain numerals, characters, and special characters. If the parameter is omitted, the card is created without a PUK.</br>DEFAULT Specifies the default PUK of 12345678.</br>PROMPT Prompts to the user to enter a PUK at the command line.`, ``),
        new Parameter(`/generate`, `Generates the files in storage that are necessary for the virtual smart card to function. If the /generate parameter is omitted, it is equivalent to creating a card without this file system. A card without a file system can be managed only by a smart card management system such as Microsoft Configuration Manager.`, ``),
        new Parameter(`/machine`, `Allows you to specify the name of a remote computer on which the virtual smart card can be created. This can be used in a domain environment only, and it relies on DCOM. For the command to succeed in creating a virtual smart card on a different computer, the user running this command must be a member in the local administrators group on the remote computer.`, ``),
        new Parameter(`/?`, `Displays Help for this command.`, ``),
    ], `The Tpmvscmgr command-line tool allows users with Administrative credentials to create and delete TPM virtual smart cards on a computer. For examples of how this command can be used, see [Examples](#BKMK_Examples).`, `Tpmvscmgr create [/name] [/AdminKey DEFAULT | PROMPT | RANDOM] [/PIN DEFAULT | PROMPT] [/PUK DEFAULT | PROMPT] [/generate] [/machine] [/?]`, "", () => { }),
    new ConsoleCommand(`tracerpt`, [
        new Parameter(`-?`, `Displays context sensitive help.`, ``),
        new Parameter(`-config <filename>`, `Load a settings file containing command options.`, ``),
        new Parameter(`-y`, `Answer yes to all questions without prompting.`, ``),
        new Parameter(`-f <XML `, ` HTML>`, ``),
        new Parameter(`-of <CSV `, ` EVTX `, ``),
        new Parameter(`-df <filename>`, `Create a Microsoft-specific counting/reporting schema file.`, ``),
        new Parameter(`-int <filename>`, `Dump the interpreted event structure to the specified file.`, ``),
        new Parameter(`-rts`, `Report raw timestamp in the event trace header. Can only be used with -o, not -report or -summary.`, ``),
        new Parameter(`-tmf <filename>`, `Specify a Trace Message Format definition file.`, ``),
        new Parameter(`-tp <value>`, `Specify the TMF file search path. Multiple paths may be used, separated by a semicolon (;).`, ``),
        new Parameter(`-i <value>`, `Specify the provider image path. The matching PDB will be located in the Symbol Server. Multiple paths can be used, separated by a semicolon (;).`, ``),
        new Parameter(`-pdb <value>`, `Specify the symbol server path. Multiple paths can be used, separated by a semicolon (;).`, ``),
        new Parameter(`-gmt`, `Convert WPP payload timestamps to Greenwich Mean Time.`, ``),
        new Parameter(`-rl <value>`, `Define System Report Level from 1 to 5. Default is 1.`, ``),
        new Parameter(`-summary [filename]`, `Generate a summary report text file. Filename if not specified is summary.txt.`, ``),
        new Parameter(`-o [filename]`, `Generate a text output file. Filename if not specified is dumpfile.xml.`, ``),
        new Parameter(`-report [filename]`, `Generate a text output report file. Filename if not specified is workload.xml.`, ``),
        new Parameter(`-lr`, `Specify "less restrictive." This uses best efforts for events that do not match the events schema.`, ``),
        new Parameter(`-export [filename]`, `Generate an Event Schema export file. Filename if not specified is schema.man.`, ``),
        new Parameter(`[-l] <value [value [â€¦]]>`, `Specify the Event Trace log file to process.`, ``),
        new Parameter(`-rt <session_name [session_name [â€¦]]>`, `Specify Real-time Event Trace Session data sources.`, ``),
    ], `The tracerpt command can be used to parse Event Trace Logs, log files generated by Performance Monitor, and real-time Event Trace providers. It generates dump files, report files, and report schemas.`, `tracerpt <[-l] <value [value [...]]>|-rt <session_name [session_name [...]]>> [options]`, "", () => { }),
    new ConsoleCommand(`tracert`, [
        new Parameter(`/d`, `Prevents tracert from attempting to resolve the IP addresses of intermediate routers to their names. This can speed up the display of tracert results.`, ``),
        new Parameter(`/h <MaximumHops>`, `Specifies the maximum number of hops in the path to search for the target (destination). The default is 30 hops.`, ``),
        new Parameter(`/j <Hostlist>`, `Specifies that echo Request messages use the Loose Source Route option in the IP header with the set of intermediate destinations specified in *Hostlist*. With loose source routing, successive intermediate destinations can be separated by one or multiple routers. The maximum number of addresses or names in the host list is 9. The *Hostlist* is a series of IP addresses (in dotted decimal notation) separated by spaces. Use this parameter only when tracing IPv4 addresses.`, ``),
        new Parameter(`/w <timeout>`, `Specifies the amount of time in milliseconds to wait for the ICMP time Exceeded or echo Reply message corresponding to a given echo Request message to be received. If not received within the time-out, an asterisk (*) is displayed. The default time-out is 4000 (4 seconds).`, ``),
        new Parameter(`/R`, `Specifies that the IPv6 Routing extension header be used to send an echo Request message to the local host, using the destination as an intermediate destination and testing the reverse route.`, ``),
        new Parameter(`/S <Srcaddr>`, `Specifies the source address to use in the echo Request messages. Use this parameter only when tracing IPv6 addresses.`, ``),
        new Parameter(`/4`, `Specifies that tracert.exe can use only IPv4 for this trace.`, ``),
        new Parameter(`/6`, `Specifies that tracert.exe can use only IPv6 for this trace.`, ``),
        new Parameter(`<TargetName>`, `Specifies the destination, identified either by IP address or host name.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Determines the path taken to a destination by sending Internet Control Message Protocol (ICMP) echo Request or ICMPv6 messages to the destination with incrementally increasing time to Live (TTL) field values. The path displayed is the list of near/side router interfaces of the routers in the path between a source host and a destination. The near/side interface is the interface of the router that is closest to the sending host in the path. Used without parameters, tracert displays help.   `, `tracert [/d] [/h <MaximumHops>] [/j <Hostlist>] [/w <timeout>] [/R] [/S <Srcaddr>] [/4][/6] <TargetName>`, "", () => { }),
    new ConsoleCommand(`tree`, [
        new Parameter(`<Drive>:`, `Specifies the drive that contains the disk for which you want to display the directory structure.`, ``),
        new Parameter(`<Path>`, `Specifies the directory for which you want to display the directory structure.`, ``),
        new Parameter(`/f`, `Displays the names of the files in each directory.`, ``),
        new Parameter(`/a`, `Specifies that tree is to use text characters instead of graphic characters to show the lines that link subdirectories.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays the directory structure of a path or of the disk in a drive graphically.`, `tree [<Drive>:][<Path>] [/f] [/a]`, "", () => { }),
    new ConsoleCommand(`tscon`, [
        new Parameter(`<SessionID>`, `Specifies the ID of the session to which you want to connect. If you use the optional /dest:<*SessionName*> parameter, this is the ID of the session to which you want to connect.`, ``),
        new Parameter(`<SessionName>`, `Specifies the name of the session to which you want to connect.`, ``),
        new Parameter(`/dest:<SessionName>`, `Specifies the name of the current session. This session will disconnect when you connect to the new session.`, ``),
        new Parameter(`/password:<pw>`, `Specifies the password of the user who owns the session to which you want to connect. This password is required when the connecting user does not own the session.`, ``),
        new Parameter(`/password:*`, `prompts for the password of the user who owns the session to which you want to connect.`, ``),
        new Parameter(`/v`, `Displays information about the actions being performed.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Connects to another session on a Remote Desktop Session Host (rd Session Host) server.  `, `tscon {<SessionID> | <SessionName>} [/dest:<SessionName>] [/password:<pw> | /password:*] [/v]`, "", () => { }),
    new ConsoleCommand(`tsdiscon`, [
        new Parameter(`<SessionId>`, `Specifies the ID of the session to disconnect.`, ``),
        new Parameter(`<SessionName>`, `Specifies the name of the session to disconnect.`, ``),
        new Parameter(`/server:<ServerName>`, `Specifies the terminal server that contains the session that you want to disconnect. Otherwise, the current rd Session Host server is used.`, ``),
        new Parameter(`/v`, `Displays information about the actions being performed.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Disconnects a session from a Remote Desktop Session Host (rd Session Host) server.`, `tsdiscon [<SessionID> | <SessionName>] [/server:<ServerName>] [/v]`, "", () => { }),
    new ConsoleCommand(`tsecimp`, [
        new Parameter(`/f <Filename>`, `Required. Specifies the name of the XML file that contains the assignment information that you want to import.`, ``),
        new Parameter(`/v`, `Validates the structure of the XML file without importing the information into the Tsec.ini file.`, ``),
        new Parameter(`/u`, `Checks whether each user is a member of the domain specified in the XML file. The computer on which you use this parameter must be connected to the network. This parameter might significantly slow performance if you are processing a large amount of user assignment information.`, ``),
        new Parameter(`/d`, `Displays a list of installed telephony providers. For each telephony provider, the associated line devices are listed, as well as the addresses and users associated with each line device.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Imports assignment information from an Extensible Markup Language (XML) file into the TAPI server security file (Tsec.ini). You can also use this command to display the list of TAPI providers and the lines devices associated with each of them, validate the structure of the XML file without importing the contents, and check domain membership.`, `tsecimp /f <Filename> [{/v | /u}]

tsecimp /d`, "", () => { }),
    new ConsoleCommand(`tskill`, [
        new Parameter(`<ProcessID>`, `Specifies the ID of the process that you want to end.`, ``),
        new Parameter(`<ProcessName>`, `Specifies the name of the process that you want to end. This parameter can include wildcard characters.`, ``),
        new Parameter(`/server:<ServerName>`, `Specifies the terminal server that contains the process that you want to end. If /server is not specified, the current RD Session Host server is used.`, ``),
        new Parameter(`/id:<SessionID>`, `Ends the process that is running in the specified session.`, ``),
        new Parameter(`/a`, `Ends the process that is running in all sessions.`, ``),
        new Parameter(`/v`, `Displays information about the actions being performed.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Ends a process running in a session on a Remote Desktop Session Host (rd Session Host) server.`, `tskill {<ProcessID> | <ProcessName>} [/server:<ServerName>] [/id:<SessionID> | /a] [/v]`, "", () => { }),
    new ConsoleCommand(`tsprof`, [
        new Parameter(`/update`, `Updates profile path information for <*UserName*> in domain <*DomainName*> to <*Profilepath*>.`, ``),
        new Parameter(`/domain:<DomainName>`, `Specifies the name of the domain in which the operation is applied.`, ``),
        new Parameter(`/local`, `Applies the operation only to local user accounts.`, ``),
        new Parameter(`/profile:<path>`, `Specifies the profile path as displayed in the Remote Desktop Services extensions in Local Users and Groups and active directory Users and computers.`, ``),
        new Parameter(`<UserName>`, `Specifies the name of the user for whom you want to update or query the server profile path.`, ``),
        new Parameter(`/copy`, `Copies user configuration information from <*SourceUser*> to <*DestinationUser*> and updates the profile path information for <*DestinationUser*> to <*Profilepath*>. Both <*SourceUser*> and <*DestinationUser*> must either be local or must be in domain <*DomainName*>.`, ``),
        new Parameter(`<Src_usr>`, `Specifies the name of the user from whom you want to copy the user configuration information.`, ``),
        new Parameter(`<Dest_usr>`, `Specifies the name of the user to whom you want to copy the user configuration information.`, ``),
        new Parameter(`/q`, `Displays the current profile path of the user for whom you want to query the server profile path.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Copies the Remote Desktop Services user configuration information from one user to another.`, `tsprof /update {/domain:<DomainName> | /local} /profile:<path> <UserName>

tsprof /copy {/domain:<DomainName> | /local} [/profile:<path>] <Src_usr> <Dest_usr>

tsprof /q {/domain:<DomainName> | /local} <UserName>`, "", () => { }),
    new ConsoleCommand(`type`, [
        new Parameter(`[<Drive>:][<Path>]<FileName>`, `Specifies the location and name of the file or files that you want to view. Separate multiple file names with spaces.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `In the Windows Command shell, type is a built in command which displays the contents of a text file. Use the type command to view a text file without modifying it.`, `type [<Drive>:][<Path>]<FileName>`, "", () => { }),
    new ConsoleCommand(`typeperf`, [
        new Parameter(`<counter [counter [â€¦]]>`, `Specifies performance counters to monitor.`, ``),
    ], `The typeperf command writes performance data to the command window or to a log file. To stop typeperf, press CTRL+C.`, `typeperf <counter [counter ...]> [options]

typeperf -cf <filename> [options]

typeperf -q [object] [options]

typeperf -qx [object] [options]`, "", () => { }),
    new ConsoleCommand(`tzutil`, [
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
        new Parameter(`/g`, `Displays the current time zone ID.`, ``),
        new Parameter(`/s <timeZoneID>[_dstoff]`, `Sets the current time zone using the specified time zone ID. The _dstoff suffix disables Daylight Saving time adjustments for the time zone (where applicable).`, ``),
        new Parameter(`/l`, `lists all valid time zone IDs and display names. The output will be:<br /><br />-   <display name><br />-   <time zone ID>`, ``),
    ], `Displays the Windows time Zone Utility. `, `tzutil [/?] [/g] [/s <timeZoneID>[_dstoff]] [/l]`, "", () => { }),
    new ConsoleCommand(`Vssadmin`, [
        new Parameter(`[Vssadmin add shadowstorage](https://docs.microsoft.com/previous-versions/windows/it-pro/windows-server-2012-r2-and-2012/cc788051(v%3dws.11))`, `Adds a volume shadow copy storage association.`, ``),
        new Parameter(`[Vssadmin create shadow](https://docs.microsoft.com/previous-versions/windows/it-pro/windows-server-2012-r2-and-2012/cc788055(v%3dws.11))`, `Creates a new volume shadow copy.`, ``),
        new Parameter(`[Vssadmin delete shadows](vssadmin-delete-shadows.md)`, `Deletes volume shadow copies.`, ``),
        new Parameter(`[Vssadmin delete shadowstorage](https://docs.microsoft.com/previous-versions/windows/it-pro/windows-server-2012-r2-and-2012/cc785461(v%3dws.11))`, `Deletes volume shadow copy storage associations.`, ``),
        new Parameter(`[Vssadmin list providers](https://docs.microsoft.com/previous-versions/windows/it-pro/windows-server-2012-r2-and-2012/cc788108(v%3dws.11))`, `Lists registered volume shadow copy providers.`, ``),
        new Parameter(`[Vssadmin list shadows](vssadmin-list-shadows.md)`, `Lists existing volume shadow copies.`, ``),
        new Parameter(`[Vssadmin list shadowstorage](https://docs.microsoft.com/previous-versions/windows/it-pro/windows-server-2012-r2-and-2012/cc788045(v%3dws.11))`, `Lists all shadow copy storage associations on the system.`, ``),
        new Parameter(`[Vssadmin list volumes](https://docs.microsoft.com/previous-versions/windows/it-pro/windows-server-2012-r2-and-2012/cc788064(v%3dws.11))`, `Lists volumes that are eligible for shadow copies.`, ``),
        new Parameter(`[Vssadmin list writers](vssadmin-list-writers.md)`, `Lists all subscribed volume shadow copy writers on the system.`, ``),
        new Parameter(`[Vssadmin resize shadowstorage](https://docs.microsoft.com/previous-versions/windows/it-pro/windows-server-2012-r2-and-2012/cc788050(v%3dws.11))`, `Resizes the maximum size for a shadow copy storage association.`, ``),
    ], `Displays current volume shadow copy backups and all installed shadow copy writers and providers. Select a command name in the following table view its command syntax.`, ``, "", () => { }),
    new ConsoleCommand(`unlodctr`, [
        new Parameter(`<DriverName>`, `removes the Performance counter name settings and Explain text for driver or service <DriverName> from the Windows Server 2003 registry.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Removes Performance counter names and Explain text for a service or device driver from the system registry.   `, `Unlodctr <DriverName>`, "", () => { }),
    new ConsoleCommand(`ver`, [
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays the operating system version number.`, `ver`, "", () => { }),
    new ConsoleCommand(`verifier`, [
        new Parameter(`<flags>`, `Must be a number in decimal or hexadecimal, combination of bits:<br /><br />-   Value: description<br />-   bit 0: special pool checking<br />-   bit 1: force irql checking<br />-   bit 2: low resources simulation<br />-   bit 3: pool tracking<br />-   bit 4: I/O verification<br />-   bit 5: deadlock detection<br />-   bit 6: unused<br />-   bit 7: DMA verification<br />-   bit 8: security checks<br />-   bit 9: force pending I/O requests<br />-   bit 10: IRP logging<br />-   bit 11: miscellaneous checks<br /><br />for example, /flags 27 is equivalent with /flags 0x1B`, ``),
        new Parameter(`/volatile`, `Used to change the verifier settings dynamically without restarting the system. Any new settings will be lost when the system is restarted.`, ``),
        new Parameter(`<probability>`, `Number between 1 and 10,000 specifying the fault injection probability. For example, specifying 100 means a fault injection probability of 1% (100/10,000).<br /><br />if this parameter is not specified then the default probability of 6% will be used.`, ``),
        new Parameter(`<tags>`, `Specifies the pool tags that will be injected with faults, separated by space characters. If this parameter is not specified then any pool allocation can be injected with faults.`, ``),
        new Parameter(`<applications>`, `Specifies the image file name of the applications that will be injected with faults, separated by space characters. If this parameter is not specified then low resources simulation can take place in any application.`, ``),
        new Parameter(`<minutes>`, `A positive number specifying the length of the period after rebooting, in minutes, during which no fault injection will occur. If this parameter is not specified then the default length of 8 minutes will be used.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Driver verifier manager.  `, `verifier /standard /driver <name> [<name> ...]  

verifier /standard /all  

verifier [/flags <flags>] [/faults [<probability> [<tags> [<applications> [<minutes>]]]] /driver <name> [<name>...]  

verifier [/flags FLAGS] [/faults [<probability> [<tags> [<applications> [<minutes>]]]] /all  

verifier /querysettings  

verifier /volatile /flags <flags>  

verifier /volatile /adddriver <name> [<name>...]  

verifier /volatile /removedriver <name> [<name>...]  

verifier /volatile /faults [<probability> [<tags> [<applications>]]  

verifier /reset  

verifier /query  

verifier /log <LogFileName> [/interval <seconds>]`, "", () => { }),
    new ConsoleCommand(`verify`, [
        new Parameter(`[on | off]`, `Switches the verify setting on or off.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Tells cmd whether to verify that your files are written correctly to a disk. If used without parameters, verify displays the current setting.`, `verify [on | off]`, "", () => { }),
    new ConsoleCommand(`vol`, [
        new Parameter(`<Drive>:`, `Specifies the drive that contains the disk for which you want to display the volume label and serial number.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays the disk volume label and serial number, if they exist.  If used without parameters, vol displays information for the current drive.`, `vol [<Drive>:]`, "", () => { }),
    new ConsoleCommand(`waitfor`, [
        new Parameter(`/s <Computer>`, `Specifies the name or IP address of a remote computer (do not use backslashes). The default is the local computer. This parameter applies to all files and folders specified in the command.`, ``),
        new Parameter(`/u [<Domain>]<User>`, `Runs the script using the credentials of the specified user account. By default, waitfor uses the current user's credentials.`, ``),
        new Parameter(`/p [<Password>]`, `Specifies the password of the user account that is specified in the /u parameter.`, ``),
        new Parameter(`/si`, `Sends the specified signal across the network.`, ``),
        new Parameter(`/t <Timeout>`, `Specifies the number of seconds to wait for a signal. By default, waitfor waits indefinitely.`, ``),
        new Parameter(`<SignalName>`, `Specifies the signal that waitfor waits for or sends. *SignalName* is not case-sensitive.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Sends or waits for a signal on a system. Waitfor is used to synchronize computers across a network.`, `waitfor [/s <Computer> [/u [<Domain>\]<User> [/p [<Password>]]]] /si <SignalName>

waitfor [/t <Timeout>] <SignalName>`, "", () => { }),
    new ConsoleCommand(`wbadmin`, [
        new Parameter(`[Wbadmin enable backup](wbadmin-enable-backup.md)`, `Configures and enables a regularly scheduled backup.`, ``),
        new Parameter(`[Wbadmin disable backup](wbadmin-disable-backup.md)`, `Disables your daily backups.`, ``),
        new Parameter(`[Wbadmin start backup](wbadmin-start-backup.md)`, `Runs a one-time backup. If used with no parameters, uses the settings from the daily backup schedule.`, ``),
        new Parameter(`[Wbadmin stop job](wbadmin-stop-job.md)`, `Stops the currently running backup or recovery operation.`, ``),
        new Parameter(`[Wbadmin get versions](wbadmin-get-versions.md)`, `Lists details of backups recoverable from the local computer or, if another location is specified, from another computer.`, ``),
        new Parameter(`[Wbadmin get items](wbadmin-get-items.md)`, `Lists the items included in a backup.`, ``),
        new Parameter(`[Wbadmin start recovery](wbadmin-start-recovery.md)`, `Runs a recovery of the volumes, applications, files, or folders specified.`, ``),
        new Parameter(`[Wbadmin get status](wbadmin-get-status.md)`, `Shows the status of the currently running backup or recovery operation.`, ``),
        new Parameter(`[Wbadmin get disks](wbadmin-get-disks.md)`, `Lists disks that are currently online.`, ``),
        new Parameter(`[Wbadmin start systemstaterecovery](wbadmin-start-systemstaterecovery.md)`, `Runs a system state recovery.`, ``),
        new Parameter(`[Wbadmin start systemstatebackup](wbadmin-start-systemstatebackup.md)`, `Runs a system state backup.`, ``),
        new Parameter(`[Wbadmin delete systemstatebackup](wbadmin-delete-systemstatebackup.md)`, `Deletes one or more system state backups.`, ``),
        new Parameter(`[Wbadmin start sysrecovery](wbadmin-start-sysrecovery.md)`, `Runs a recovery of the full system (at least all the volumes that contain the operating system's state). This subcommand  is only available if you are using the Windows Recovery Environment.`, ``),
        new Parameter(`[Wbadmin restore catalog](wbadmin-restore-catalog.md)`, `Recovers a backup catalog from a specified storage location in the case where the backup catalog on the local computer has been corrupted.`, ``),
        new Parameter(`[Wbadmin delete catalog](wbadmin-delete-catalog.md)`, `Deletes the backup catalog on the local computer. Use this subcommand only if the backup catalog on this computer is corrupted and you have no backups stored at another location that you can use to restore the catalog.`, ``),
    ], `Enables you to back up and restore your operating system, volumes, files, folders, and applications from a command prompt.`, ``, "", () => { }),
    new ConsoleCommand(`wdsutil`, [
        new Parameter(`[Using the add Command](using-the-add-command.md)`, `adds objects or prestages computers.`, ``),
        new Parameter(`[Using the Approve-AutoaddDevices Command](using-the-approve-autoadddevices-command.md)`, `Approves computers that are pending administrator approval.`, ``),
        new Parameter(`[Using the convert-RiprepImage Command](using-the-convert-riprepimage-command.md)`, `converts an existing remote Installation Preparation (RIPrep) image to a Windows Image (.wim) file.`, ``),
        new Parameter(`[Using the copy Command](using-the-copy-command.md)`, `Copies an image or a driver group.`, ``),
        new Parameter(`[Using the delete-AutoaddDevices Command](using-the-delete-autoadddevices-command.md)`, `deletes computers that are in the Auto-add database (which stores information about the computers on the server).`, ``),
        new Parameter(`[Using the Disable Command](using-the-disable-command.md)`, `Disables all services for Windows Deployment Services.`, ``),
        new Parameter(`[Using the Disconnect-Client Command](using-the-disconnect-client-command.md)`, `Disconnects a client from a multicast transmission or namespace.`, ``),
        new Parameter(`[Using the Enable Command](using-the-enable-command.md)`, `Enables all services for Windows Deployment Services.`, ``),
        new Parameter(`[Using the Export-Image Command](using-the-export-image-command.md)`, `Exports an image from the image store to a .wim file.`, ``),
        new Parameter(`[Using The get Command](using-the-get-command.md)`, `Retrieves properties and attributes about the specified object.`, ``),
        new Parameter(`[Using the Initialize-Server Command](using-the-initialize-server-command.md)`, `Configures a Windows Deployment Services server for initial use.`, ``),
        new Parameter(`[Using the New Command](using-the-new-command.md)`, `creates new capture and discover images as well as multicast transmissions and namespaces.`, ``),
        new Parameter(`[The progress Command](the-progress-command.md)`, `Displays the progress status while a command is being executed.`, ``),
        new Parameter(`[Using The Reject-AutoaddDevices Command](using-the-reject-autoadddevices-command.md)`, `Rejects computers that are pending administrator approval.`, ``),
        new Parameter(`[Using the remove Command](using-the-remove-command.md)`, `removes objects.`, ``),
        new Parameter(`[Using the replace-Image Command](using-the-replace-image-command.md)`, `replaces a boot or installation image with a new version of that image.`, ``),
        new Parameter(`[The Set Command](the-set-command.md)`, `Sets properties and attributes on the specified object.`, ``),
        new Parameter(`[The start Server Command](the-start-server-command.md)`, `starts all services on the Windows Deployment Services server, including multicast transmissions, namespaces, and the Transport Server.`, ``),
        new Parameter(`[The Stop Server Command](the-stop-server-command.md)`, `Stops all services on the Windows Deployment Services server.`, ``),
        new Parameter(`[The uninitialize-Server Option](the-uninitialize-server-option.md)`, `reverts changes made during server initialization.`, ``),
        new Parameter(`[The Update-ServerFiles Command](the-update-serverfiles-command.md)`, `Updates server files on the remoteInstall share.`, ``),
        new Parameter(`[The verbose Command](the-verbose-command.md)`, `Displays verbose output for the specified command.`, ``),
    ], `wdsutil is a command-line utility used for managing your Windows Deployment Services server. To run these commands, click start, right-click Command prompt, and click Run as administrator.  `, ``, "", () => { }),
    new ConsoleCommand(`wecutil`, [
        new Parameter(`{es | enum-subscription}`, `Displays the names of all remote event subscriptions that exist.`, ``),
        new Parameter(`{gs | get-subscription} <Subid> [/f:<Format>] [/uni:<Unicode>]`, `Displays remote subscription configuration information. <Subid> is a string that uniquely identifies a subscription. <Subid> is the same as the string that was specified in the <SubscriptionId> tag of the XML configuration file, which was used to create the subscription.`, ``),
        new Parameter(`{gr | get-subscriptionruntimestatus} <Subid> [<Eventsource> â€¦]`, `Displays the runtime status of a subscription. <Subid> is a string that uniquely identifies a subscription. <Subid> is the same as the string that was specified in the <SubscriptionId> tag of the XML configuration file, which was used to create the subscription. <Eventsource> is a string that identifies a computer that serves as a source of events. <Eventsource> should be a fully qualified domain name, a NetBIOS name, or an IP address.`, ``),
        new Parameter(`{ss | set-subscription} <Subid> [/e:[<Subenabled>]] [/esa:<Address>] [/ese:[<Srcenabled>]] [/aes] [/res] [/un:<Username>] [/up:<Password>] [/d:<Desc>] [/uri:<Uri>] [/cm:<Configmode>] [/ex:<Expires>] [/q:<Query>] [/dia:<Dialect>] [/tn:<Transportname>] [/tp:<Transportport>]  [/dm:<Deliverymode>] [/dmi:<Deliverymax>] [/dmlt:<Deliverytime>] [/hi:<Heartbeat>] [/cf:<Content>] [/l:<Locale>] [/ree:[<Readexist>]] [/lf:<Logfile>] [/pn:<Publishername>] [/essp:<Enableport>] [/hn:<Hostname>] [/ct:<Type>]</br>or</br>{ss | set-subscription /c:<Configfile> [/cun:<Comusername> /cup:<Compassword>]`, `Changes the subscription configuration. You can specify the subscription ID and the appropriate options to change subscription parameters, or you can specify an XML configuration file to change subscription parameters.`, ``),
        new Parameter(`{cs | create-subscription} <Configfile> [/cun:<Username> /cup:<Password>]`, `Creates a remote subscription. <Configfile> specifies the path to the XML file that contains the subscription configuration. The path can be absolute or relative to the current directory.`, ``),
        new Parameter(`{ds | delete-subscription} <Subid>`, `Deletes a subscription and unsubscribes from all event sources that deliver events into the event log for the subscription. Any events already received and logged are not deleted. <Subid> is a string that uniquely identifies a subscription. <Subid> is the same as the string that was specified in the <SubscriptionId> tag of the XML configuration file, which was used to create the subscription.`, ``),
        new Parameter(`{rs | retry-subscription} <Subid> [<Eventsource>â€¦]`, `Retries to establish a connection and send a remote subscription request to an inactive subscription. Attempts to reactivate all event sources or specified event sources. Disabled sources are not retried. <Subid> is a string that uniquely identifies a subscription. <Subid> is the same as the string that was specified in the <SubscriptionId> tag of the XML configuration file, which was used to create the subscription. <Eventsource> is a string that identifies a computer that serves as a source of events. <Eventsource> should be a fully qualified domain name, a NetBIOS name, or an IP address.`, ``),
        new Parameter(`{qc | quick-config} [/q:[<Quiet>]]`, `Configures the Windows Event Collector service to ensure a subscription can be created and sustained through reboots. This includes the following steps:</br>1.  Enable the ForwardedEvents channel if it is disabled.</br>2.  Set the Windows Event Collector service to delay start.</br>3.  Start the Windows Event Collector service if it is not running.`, ``),
    ], `Enables you to create and manage subscriptions to events that are forwarded from remote computers. The remove computer must support thye WS-Management protocol. For examples of how to use this command, see [Examples](#BKMK_examples).`, `wecutil  [{es | enum-subscription}] 

[{gs | get-subscription} <Subid> [/f:<Format>] [/uni:<Unicode>]] 

[{gr | get-subscriptionruntimestatus} <Subid> [<Eventsource> â€¦]] 

[{ss | set-subscription} [<Subid> [/e:[<Subenabled>]] [/esa:<Address>] [/ese:[<Srcenabled>]] [/aes] [/res] [/un:<Username>] [/up:<Password>] [/d:<Desc>] [/uri:<Uri>] [/cm:<Configmode>] [/ex:<Expires>] [/q:<Query>] [/dia:<Dialect>] [/tn:<Transportname>] [/tp:<Transportport>] [/dm:<Deliverymode>] [/dmi:<Deliverymax>] [/dmlt:<Deliverytime>] [/hi:<Heartbeat>] [/cf:<Content>] [/l:<Locale>] [/ree:[<Readexist>]] [/lf:<Logfile>] [/pn:<Publishername>] [/essp:<Enableport>] [/hn:<Hostname>] [/ct:<Type>]] [/c:<Configfile> [/cun:<Username> /cup:<Password>]]] 

[{cs | create-subscription} <Configfile> [/cun:<Username> /cup:<Password>]] 

[{ds | delete-subscription} <Subid>] 

[{rs | retry-subscription} <Subid> [<Eventsource>â€¦]] 

[{qc | quick-config} [/q:[<Quiet>]]].`, "", () => { }),
    new ConsoleCommand(`wevtutil`, [
        new Parameter(`{el | enum-logs}`, `Displays the names of all logs.`, ``),
        new Parameter(`{gl | get-log} <Logname> [/f:<Format>]`, `Displays configuration information for the specified log, which includes whether the log is enabled or not, the current maximum size limit of the log, and the path to the file where the log is stored.`, ``),
        new Parameter(`{sl | set-log} <Logname> [/e:<Enabled>] [/i:<Isolation>] [/lfn:<Logpath>] [/rt:<Retention>] [/ab:<Auto>] [/ms:<MaxSize>] [/l:<Level>] [/k:<Keywords>] [/ca:<Channel>] [/c:<Config>]`, `Modifies the configuration of the specified log.`, ``),
        new Parameter(`{ep | enum-publishers}`, `Displays the event publishers on the local computer.`, ``),
        new Parameter(`{gp | get-publisher} <Publishername> [/ge:<Metadata>] [/gm:<Message>] [/f:<Format>]]`, `Displays the configuration information for the specified event publisher.`, ``),
        new Parameter(`{im | install-manifest} <Manifest>`, `Installs event publishers and logs from a manifest. For more information about event manifests and using this parameter, see the Windows Event Log SDK at the Microsoft Developers Network (MSDN) Web site (https://msdn.microsoft.com).`, ``),
        new Parameter(`{um | uninstall-manifest} <Manifest>`, `Uninstalls all publishers and logs from a manifest. For more information about event manifests and using this parameter, see the Windows Event Log SDK at the Microsoft Developers Network (MSDN) Web site (https://msdn.microsoft.com).`, ``),
        new Parameter(`{qe | query-events} <Path> [/lf:<Logfile>] [/sq:<Structquery>] [/q:<Query>] [/bm:<Bookmark>] [/sbm:<Savebm>] [/rd:<Direction>] [/f:<Format>] [/l:<Locale>] [/c:<Count>] [/e:<Element>]`, `Reads events from an event log, from a log file, or using a structured query. By default, you provide a log name for <Path>. However, if you use the /lf option, then <Path> must be a path to a log file. If you use the /sq parameter, <Path> must be a path to a file that contains a structured query.`, ``),
        new Parameter(`{gli | get-loginfo} <Logname> [/lf:<Logfile>]`, `Displays status information about an event log or log file. If the /lf option is used, <Logname> is a path to a log file. You can run wevtutil el to obtain a list of log names.`, ``),
        new Parameter(`{epl | export-log} <Path> <Exportfile> [/lf:<Logfile>] [/sq:<Structquery>] [/q:<Query>] [/ow:<Overwrite>]`, `Exports events from an event log, from a log file, or using a structured query to the specified file. By default, you provide a log name for <Path>. However, if you use the /lf option, then <Path> must be a path to a log file. If you use the /sq option, <Path> must be a path to a file that contains a structured query. <Exportfile> is a path to the file where the exported events will be stored.`, ``),
        new Parameter(`{al | archive-log} <Logpath> [/l:<Locale>]`, `Archives the specified log file in a self-contained format. A subdirectory with the name of the locale is created and all locale-specific information is saved in that subdirectory. After the directory and log file are created by running wevtutil al, events in the file can be read whether the publisher is installed or not.`, ``),
        new Parameter(`{cl | clear-log} <Logname> [/bu:<Backup>]`, `Clears events from the specified event log. The /bu option can be used to back up the cleared events.`, ``),
    ], `Enables you to retrieve information about event logs and publishers. You can also use this command to install and uninstall event manifests, to run queries, and to export, archive, and clear logs. For examples of how to use this command, see [Examples](#BKMK_examples).`, `wevtutil [{el | enum-logs}] [{gl | get-log} <Logname> [/f:<Format>]]

[{sl | set-log} <Logname> [/e:<Enabled>] [/i:<Isolation>] [/lfn:<Logpath>] [/rt:<Retention>] [/ab:<Auto>] [/ms:<MaxSize>] [/l:<Level>] [/k:<Keywords>] [/ca:<Channel>] [/c:<Config>]] 

[{ep | enum-publishers}] 

[{gp | get-publisher} <Publishername> [/ge:<Metadata>] [/gm:<Message>] [/f:<Format>]] [{im | install-manifest} <Manifest>] 

[{um | uninstall-manifest} <Manifest>] [{qe | query-events} <Path> [/lf:<Logfile>] [/sq:<Structquery>] [/q:<Query>] [/bm:<Bookmark>] [/sbm:<Savebm>] [/rd:<Direction>] [/f:<Format>] [/l:<Locale>] [/c:<Count>] [/e:<Element>]] 

[{gli | get-loginfo} <Logname> [/lf:<Logfile>]] 

[{epl | export-log} <Path> <Exportfile> [/lf:<Logfile>] [/sq:<Structquery>] [/q:<Query>] [/ow:<Overwrite>]] 

[{al | archive-log} <Logpath> [/l:<Locale>]] 

[{cl | clear-log} <Logname> [/bu:<Backup>]] [/r:<Remote>] [/u:<Username>] [/p:<Password>] [/a:<Auth>] [/uni:<Unicode>]`, "", () => { }),
    new ConsoleCommand(`where`, [
        new Parameter(`/r <Dir>`, `Indicates a recursive search, starting with the specified directory.`, ``),
        new Parameter(`/q`, `Returns an exit code (0 for success, 1 for failure) without displaying the list of matched files.`, ``),
        new Parameter(`/f`, `Displays the results of the where command in quotation marks.`, ``),
        new Parameter(`/t`, `Displays the file size and the last modified date and time of each matched file.`, ``),
        new Parameter(`[$<ENV>:|<Path>:]<Pattern>[ ...]`, `Specifies the search pattern for the files to match. At least one pattern is required, and the pattern can include wildcard characters (* and ?). By default, where searches the current directory and the paths that are specified in the PATH environment variable. You can specify a different path to search by using the format $*ENV*:*Pattern* (where *ENV* is an existing environment variable containing one or more paths) or by using the format *Path*:*Pattern* (where *Path* is the directory path you want to search). These optional formats should not be used with the /r command-line option.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays the location of files that match the given search pattern.`, `where [/r <Dir>] [/q] [/f] [/t] [$<ENV>:|<Path>:]<Pattern>[ ...]`, "", () => { }),
    new ConsoleCommand(`whoami`, [
        new Parameter(`/upn`, `Displays the user name in user principal name (UPN) format.`, ``),
        new Parameter(`/fqdn`, `Displays the user name in fully qualified domain name (FQDN) format.`, ``),
        new Parameter(`/logonid`, `Displays the logon ID of the current user.`, ``),
        new Parameter(`/user`, `Displays the current domain and user name and the security identifier (SID).`, ``),
        new Parameter(`/groups`, `Displays the user groups to which the current user belongs.`, ``),
        new Parameter(`/priv`, `Displays the security privileges of the current user.`, ``),
        new Parameter(`/fo <Format>`, `Specifies the output format. Valid values include:</br>table Displays output in a table. This is the default value.</br>list Displays output in a list.</br>csv Displays output in comma-separated value (CSV) format.`, ``),
        new Parameter(`/all`, `Displays all information in the current access token, including the current user name, security identifiers (SID), privileges, and groups that the current user belongs to.`, ``),
        new Parameter(`/nh`, `Specifies that the column header should not be displayed in the output. This is valid only for table and CSV formats.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Displays user, group and privileges information for the user who is currently logged on to the local system. If used without parameters, whoami displays the current domain and user name.`, `whoami [/upn | /fqdn | /logonid]

whoami {[/user] [/groups] [/priv]} [/fo <Format>] [/nh]

whoami /all [/fo <Format>] [/nh]`, "", () => { }),
    new ConsoleCommand(`winnt`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Winnt is deprecated, and is not guaranteed to be supported in future releases of Windows.`, ``, "", () => { }),
    new ConsoleCommand(`winnt32`, [
        new Parameter(`/checkupgradeonly`, `Checks your computer for upgrade compatibility with products in Windows Server 2003.<br /><br />if you use this option with /unattend, no user input is required.  Otherwise, the results are displayed on the screen, and you can save them under the file name you specify. The default file name is upgrade.txt in the systemroot folder.`, ``),
        new Parameter(`/cmd`, `Instructs setup to carry out a specific command before the final phase of setup. This occurs after your computer has restarted and after setup has collected the necessary configuration information, but before setup is complete.`, ``),
        new Parameter(`<CommandLine>`, `Specifies the commandline to be carried out before the final phase of setup.`, ``),
        new Parameter(`/cmdcons`, `On an x86-based computer, installs the recovery Console as a startup option.  The recovery Console is a command-line interface from which you can perform tasks such as starting and stopping services and accessing the local drive (including drives formatted with NTFS). You can only use the /cmdcons option after setup is finished.`, ``),
        new Parameter(`/copydir`, `creates an additional folder within the folder in which the operating system files are installed.  for example, for x86 and x64-based computers, you could create a folder called *Private_drivers* within the i386 source folder for your installation, and place driver files in the folder. type /copydir:i386\*Private_drivers* to have setup copy that folder to  your newly installed computer, making the new folder location systemroot\*Private_drivers*.<br /><br />-   i386 specifies i386<br />-   ia64 specifies ia64<br /><br />You can use /copydir to create as many additional folders as you want.`, ``),
        new Parameter(`<FolderName>`, `Specifies the folder that you created to hold modifications for your site.`, ``),
        new Parameter(`/copysource`, `creates a temporary additional folder within the folder in which the operating system files are installed. You can use /copysource to create as many additional folders as you want.<br /><br />Unlike the folders /copydir creates, /copysource folders are deleted after Setup completes.`, ``),
        new Parameter(`/debug`, `creates a debug log at the level specified, for example, /debug4:Debug.log.  The default log file is C: systemrootwinnt32.log, and`, ``),
        new Parameter(`<level>`, `Level Values and descriptions<br /><br />-   0: Severe Errors<br />-   1: Errors<br />-   2: Default level. Warnings<br />-   3: Information<br />-   4: detailed information for debugging<br /><br />Each level includes the levels below it.`, ``),
        new Parameter(`/dudisable`, `Prevents Dynamic Update from running. Without Dynamic Update, setup runs only with the original setup files. This option will disable Dynamic Update even if you use an answer file and specify Dynamic Update options in that file.`, ``),
        new Parameter(`/duprepare`, `Carries out preparations on an installation share so that it can be used with Dynamic Update files that you downloaded from the Windows Update Web site. This share can then be used for installing Windows XP for multiple clients.`, ``),
        new Parameter(`<pathName>`, `Specifies full path name.`, ``),
        new Parameter(`/dushare`, `Specifies a share on which you previously downloaded Dynamic Update files (updated files for use with Setup) from the Windows Update Web site, and on which you previously ran /duprepare:*< pathName>*. When run on a client, specifies that the client installation will make use of the updated files on the share specified in <pathName>.`, ``),
        new Parameter(`/emsport`, `Enables or disables Emergency Management Services during setup and after the server operating system has been installed. With Emergency Management Services, you can remotely manage a server in emergency situations that would typically require a local keyboard, mouse, and monitor, such as when the network is unavailable or the server is not functioning properly. Emergency Management Services has specific hardware requirements, and is available only for products in Windows Server 2003.<br /><br />-   com1 is applicable only for x86-based computers (not Itanium architecture-based computers).<br />-   com2is applicable only for x86-based computers (not Itanium architecture-based computers).<br />-   Default. Uses the setting specified in the BIOS Serial Port Console Redirection (SPCR) table, or, in Itanium architecture-based systems, through the EFI console device path. If you specify usebiossettings and there is no SPCR table or appropriate EFI console device path, Emergency Management Serices will not be enabled.<br />-   off disables Emergency Management Services. You can later enable it by modifying the boot settings.`, ``),
        new Parameter(`/emsbaudrate`, `for x86-based computers, specifies the baud rate for Emergency Management Services. (The option is not applicable for Itanium architecture-based computers.) Must be used with /emsport:com1 or /emsport:com2 (otherwise,  /emsbaudrate is ignored).`, ``),
        new Parameter(`<BaudRate>`, `Specifies baudrate of 9600, 19200, 57600, or 115200. 9600 is the default.`, ``),
        new Parameter(`/m`, `Specifies that setup copies replacement files from an alternate location.  Instructs setup to look in the alternate location first, and if files are present, to use them instead of the files from the default location.`, ``),
        new Parameter(`/makelocalsource`, `Instructs setup to copy all installation source files to your local hard disk.  Use /makelocalsource when installing from a cd to provide installation files when the cd is not available later in the installation.`, ``),
        new Parameter(`/noreboot`, `Instructs setup to not restart the computer after the file copy phase of setup is completed so that you can run another command.`, ``),
        new Parameter(`/s`, `Specifies the source location of the files for your installation. To simultaneously copy files from multiple servers, type the /s:<Sourcepath> option multiple times (up to a maximum of eight). If you type the option multiple times, the first server specified must be available, or setup will fail.`, ``),
        new Parameter(`<Sourcepath>`, `Specifies full source path name.`, ``),
        new Parameter(`/syspart`, `On an x86-based computer, specifies that you can copy setup startup files to a hard disk, mark the disk as active, and then install the disk into another computer. When you start that computer, it automatically starts with the next phase of setup.<br /><br />You must always use the /tempdrive parameter with the /syspart parameter.<br /><br />You can start winnt32 with the /syspart option on an x86-based computer running Windows NT 4.0, Windows 2000, Windows XP, or a product in Windows Server 2003. If the computer is running Windows NT version 4.0, it requires Service Pack 5 or later. The computer cannot be running Windows 95, Windows 98, or Windows Millennium edition.`, ``),
        new Parameter(`<DriveLetter>`, `Specifies the drive letter.`, ``),
        new Parameter(`/tempdrive`, `directs setup to place temporary files on the specified partition.<br /><br />for a new installation, the server operating system will also be installed on the specified partition.<br /><br />for an upgrade, the /tempdrive option affects the placement of temporary files only; the operating system will be upgraded in the partition from which you run winnt32.`, ``),
        new Parameter(`/udf`, `Indicates an identifier (<ID>) that setup uses to specify how a Uniqueness Database (UDB) file modifies an answer file (see the /unattend option).  The UDB overrides values in the answer file, and the identifier determines which values in the UDB file are used. For example, /udf:RAS_user,Our_company.udb overrides settings specified for the RAS_user identifier in the Our_company.udb  file. If no <UDB_file> is specified, setup prompts the user to insert a disk that contains the $Unique$.udb file.`, ``),
        new Parameter(`<ID>`, `Indicates an identifier used to specify how a Uniqueness Database (UDB) file modifies an answer file.`, ``),
        new Parameter(`<UDB_file>`, `Specifies a Uniqueness Database (UDB) file.`, ``),
        new Parameter(`/unattend`, `On an x86-based computer, upgrades your previous version of Windows NT 4.0 Server (with Service Pack 5 or later) or Windows 2000 in unattended setup mode. All user settings are taken from the previous installation, so no user intervention is required during setup.`, ``),
        new Parameter(`<num>`, `Specifies the number of seconds between the time that setup finishes copying the files and when it restarts your computer. You can use <Num> on any computer running Windows 98, Windows Millennium edition, Windows NT, Windows 2000, Windows XP, or a product in Windows Server 2003 . If the computer is running Windows NT version 4.0, it requires Service Pack 5 or later.`, ``),
        new Parameter(`<AnswerFile>`, `Provides setup with your custom specifications`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Performs an installation of or upgrade to a product in Windows Server 2003. You can run winnt32 at the command prompt on a computer running Windows 95, Windows 98, Windows Millennium edition, Windows NT, Windows 2000, Windows XP, or a product in the Windows Server 2003. If you run winnt32 on a computer running Windows NT version 4.0, you must first apply Service Pack 5 or later.`, `winnt32 [/checkupgradeonly] [/cmd: <CommandLine>] [/cmdcons] [/copydir:{i386|ia64}\<FolderName>] [/copysource: <FolderName>] [/debug[<Level>]:[ <FileName>]] [/dudisable] [/duprepare: <pathName>] [/dushare: <pathName>] [/emsport:{com1|com2|usebiossettings|off}] [/emsbaudrate: <BaudRate>] [/m: <FolderName>]  [/makelocalsource] [/noreboot] [/s: <Sourcepath>] [/syspart: <DriveLetter>] [/tempdrive: <DriveLetter>] [/udf: <ID>[,<UDB_File>]] [/unattend[<Num>]:[ <AnswerFile>]]`, "", () => { }),
    new ConsoleCommand(`winpop`, [
        new Parameter(`t`, `i`, ``),
        new Parameter(`d`, `e`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`a`, `u`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`m`, `a`, ``),
        new Parameter(`m`, `s`, ``),
        new Parameter(`-`, `-`, ``),
    ], `Winpop is deprecated, and is not guaranteed to be supported in future releases of Windows.`, ``, "", () => { }),
    new ConsoleCommand(`winrs`, [
        new Parameter(`[/remote]:<endpoint>`, `Specifies the target endpoint using a NetBIOS name or the standard connection:<br /><br />-   <url>: [<transport>://]<target>[:<port>]<br /><br />if not specified, /r:localhost is used.`, ``),
        new Parameter(`/unencrypted]`, `Specifies that the messages to the remote shell will not be encrypted. This is useful for troubleshooting or when the network traffic is already encrypted using ipsec, or when physical security is enforced.<br /><br />By default, the messages are encrypted using Kerberos or NTLM keys.<br /><br />This command-line option is ignored when HTTPS transport is selected.`, ``),
        new Parameter(`/username]:<username>`, `Specifies username on command line.<br /><br />if not specified, the tool will use Negotiate authentication or prompt for the name.<br /><br />if /username is specified, /password must also be specified.`, ``),
        new Parameter(`/password]:<password>`, `Specifies password on command line.<br /><br />if /password is not specified but /username is, the tool will prompt for the password.<br /><br />if /password is specified, /username must also be specified.`, ``),
        new Parameter(`/timeout:<seconds>`, `This option is deprecated.`, ``),
        new Parameter(`/directory:<path>`, `Specifies starting directory for remote shell.<br /><br />if not specified, the remote shell will start in the user's home directory defined by the environment variable %USERPROFILE%.`, ``),
        new Parameter(`/environment:<string>=<value>`, `Specifies a single environment variable to be set when shell starts, which allows changing default environment for shell.<br /><br />Multiple occurrences of this switch must be used to specify multiple environment variables.`, ``),
        new Parameter(`/noecho`, `Specifies that echo should be disabled. This may be necessary to ensure that user's answers to remote prompts are not displayed locally.<br /><br />By default echo is "on".`, ``),
        new Parameter(`/noprofile`, `Specifies that the user's profile should not be loaded.<br /><br />By default, the server will attempt to load the user profile.<br /><br />if the remote user is not a local administrator on the target system, then this option will be required (the default will result in error).`, ``),
        new Parameter(`/allowdelegate`, `Specifies that the user's credentials can be used to access a remote share, for example, found on a different machine than the target endpoint.`, ``),
        new Parameter(`/compression`, `Turn on compression.  Older installations on remote machines may not support compression so it is off by default.<br /><br />Default setting is off, since older installations on remote machines may not support compression.`, ``),
        new Parameter(`/usessl`, `Use an SSL connection when using a remote endpoint.  Specifying this instead of the transport https: will use the default WinRM default port.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Windows remote Management allows you to manage and execute programs remotely.   `, `winrs [/<parameter>[:<value>]] <command>`, "", () => { }),
    new ConsoleCommand(`wmic`, [
        new Parameter(`class`, `Escapes from the default alias mode of WMIC to access classes in the WMI schema directly.`, ``),
        new Parameter(`path`, `Escapes from the default alias mode of WMIC to access instances in the WMI schema directly.`, ``),
        new Parameter(`context`, `Displays the current values of all global switches.`, ``),
        new Parameter(`[quit | exit]`, `Exits the WMIC command shell.`, ``),
    ], `Displays WMI information inside an interactive command shell.`, `command </parameter>`, "", () => { }),
    new ConsoleCommand(`wscript`, [
        new Parameter(`scriptname`, `Specifies the path and file name of the script file.`, ``),
        new Parameter(`/b`, `Specifies batch mode, which does not display alerts, scripting errors, or input prompts. This is the opposite of /i.`, ``),
        new Parameter(`/d`, `Starts the debugger.`, ``),
        new Parameter(`/e`, `Specifies the engine that is used to run the script. This lets you run scripts that use a custom file name extension. Without the /e parameter, you can only run scripts that use registered file name extensions. For example, if you try to run this command:<br>"""cscript test.admin"""<br>You will receive this error message: Input Error: There is no script engine for file extension ".admin."<br>One advantage of using nonstandard file name extensions is that it guards against accidentally double-clicking a script and running something you really did not want to run. <br>This does not create a permanent association between the .admin file name extension and VBScript. Each time you run a script that uses a .admin file name extension, you will need to use the /e parameter.`, ``),
        new Parameter(`/h:cscript`, `Registers cscript.exe as the default script host for running scripts.`, ``),
        new Parameter(`/h:wscript`, `Registers wscript.exe as the default script host for running scripts. This is the default when the /h option is omitted.`, ``),
        new Parameter(`/i`, `Specifies interactive mode, which displays alerts, scripting errors, and input prompts.</br>This is the default and the opposite of /b.`, ``),
        new Parameter(`/job:<identifier>`, `Runs the job identified by *identifier* in a .wsf script file.`, ``),
        new Parameter(`/logo`, `Specifies that the Windows Script Host banner is displayed in the console before the script runs.</br>This is the default and the opposite of /nologo.`, ``),
        new Parameter(`/nologo`, `Specifies that the Windows Script Host banner is not displayed before the script runs. This is the opposite of /logo.`, ``),
        new Parameter(`/s`, `Saves the current command prompt options for the current user.`, ``),
        new Parameter(`/t:<number>`, `Specifies the maximum time the script can run (in seconds). You can specify up to 32,767 seconds.</br>The default is no time limit.`, ``),
        new Parameter(`/x`, `Starts the script in the debugger.`, ``),
        new Parameter(`ScriptArguments`, `Specifies the arguments passed to the script. Each script argument must be preceded by a slash (/).`, ``),
        new Parameter(`/?`, `Displays Help at the command prompt.`, ``),
    ], `Windows Script Host provides an environment in which users can execute scripts in a variety of languages that use a variety of object models to perform tasks.`, `wscript [<scriptname>] [/b] [/d] [/e:<engine>] [{/h:cscript|/h:wscript}] [/i] [/job:<identifier>] [{/logo|/nologo}] [/s] [/t:<number>] [/x] [/?] [<ScriptArguments>]`, "", () => { }),
    new ConsoleCommand(`xcopy`, [
        new Parameter(`<Source>`, `Required. Specifies the location and names of the files you want to copy. This parameter must include either a drive or a path.`, ``),
        new Parameter(`[<Destination>]`, `Specifies the destination of the files you want to copy. This parameter can include a drive letter and colon, a directory name, a file name, or a combination of these.`, ``),
        new Parameter(`/w`, `Displays the following message and waits for your response before starting to copy files:</br>Press any key to begin copying file(s)`, ``),
        new Parameter(`/p`, `Prompts you to confirm whether you want to create each destination file.`, ``),
        new Parameter(`/c`, `Ignores errors.`, ``),
        new Parameter(`/v`, `Verifies each file as it is written to the destination file to make sure that the destination files are identical to the source files.`, ``),
        new Parameter(`/q`, `Suppresses the display of xcopy messages.`, ``),
        new Parameter(`/f`, `Displays source and destination file names while copying.`, ``),
        new Parameter(`/l`, `Displays a list of files that are to be copied.`, ``),
        new Parameter(`/g`, `Creates decrypted *Destination* files when the destination does not support encryption.`, ``),
        new Parameter(`/d [:MM-DD-YYYY]`, `Copies source files changed on or after the specified date only. If you do not include a *MM-DD-YYYY* value, xcopy copies all *Source* files that are newer than existing *Destination* files. This command-line option allows you to update files that have changed.`, ``),
        new Parameter(`/u`, `Copies files from *Source* that exist on *Destination* only.`, ``),
        new Parameter(`/i`, `If *Source* is a directory or contains wildcards and *Destination* does not exist, xcopy assumes *Destination* specifies a directory name and creates a new directory. Then, xcopy copies all specified files into the new directory. By default, xcopy prompts you to specify whether *Destination* is a file or a directory.`, ``),
        new Parameter(`/s`, `Copies directories and subdirectories, unless they are empty. If you omit /s, xcopy works within a single directory.`, ``),
        new Parameter(`/e`, `Copies all subdirectories, even if they are empty. Use /e with the /s and /t command-line options.`, ``),
        new Parameter(`/t`, `Copies the subdirectory structure (that is, the tree) only, not files. To copy empty directories, you must include the /e command-line option.`, ``),
        new Parameter(`/k`, `Copies files and retains the read-only attribute on *Destination* files if present on the *Source* files. By default, xcopy removes the read-only attribute.`, ``),
        new Parameter(`/r`, `Copies read-only files.`, ``),
        new Parameter(`/h`, `Copies files with hidden and system file attributes. By default, xcopy does not copy hidden or system files`, ``),
        new Parameter(`/a`, `Copies only *Source* files that have their archive file attributes set. /a does not modify the archive file attribute of the source file. For information about how to set the archive file attribute by using attrib, see [Additional references](xcopy.md#BKMK_addref).`, ``),
        new Parameter(`/m`, `Copies *Source* files that have their archive file attributes set. Unlike /a, /m turns off archive file attributes in the files that are specified in the source. For information about how to set the archive file attribute by using attrib, see [Additional references](xcopy.md#BKMK_addref).`, ``),
        new Parameter(`/n`, `Creates copies by using the NTFS short file or directory names. /n is required when you copy files or directories from an NTFS volume to a FAT volume or when the FAT file system naming convention (that is, 8.3 characters) is required on the *Destination* file system. The *Destination* file system can be FAT or NTFS.`, ``),
        new Parameter(`/o`, `Copies file ownership and discretionary access control list (DACL) information.`, ``),
        new Parameter(`/x`, `Copies file audit settings and system access control list (SACL) information (implies /o).`, ``),
        new Parameter(`/exclude:FileName1[+[FileName2][+[FileName3]( )]`, `Specifies a list of files. At least one file must be specified. Each file will contain search strings with each string on a separate line in the file.</br>When any of the strings match any part of the absolute path of the file to be copied, that file will be excuded from being copied. For example, specifying the string obj will exclude all files underneath the directory obj or all files with the .obj extension.`, ``),
        new Parameter(`/y`, `Suppresses prompting to confirm that you want to overwrite an existing destination file.`, ``),
        new Parameter(`/-y`, `Prompts to confirm that you want to overwrite an existing destination file.`, ``),
        new Parameter(`/z`, `Copies over a network in restartable mode.`, ``),
        new Parameter(`/b`, `Copies the symbolic link instead of the files. This parameter was introduced in Windows VistaÂ®.`, ``),
        new Parameter(`/j`, `Copies files without buffering. Recommended for very large files. This parameter was added in Windows Server 2008 R2.`, ``),
        new Parameter(`/?`, `Displays help at the command prompt.`, ``),
    ], `Copies files and directories, including subdirectories`, `Xcopy <Source> [<Destination>] [/w] [/p] [/c] [/v] [/q] [/f] [/l] [/g] [/d [:MM-DD-YYYY]] [/u] [/i] [/s [/e]] [/t] [/k] [/r] [/h] [{/a | /m}] [/n] [/o] [/x] [/exclude:FileName1[+[FileName2]][+[FileName3]] [{/y | /-y}] [/z] [/b] [/j]`, "", () => { }),
];
export const CMDCommands = commands;
//# sourceMappingURL=CMDCommands.js.map