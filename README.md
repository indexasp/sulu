SULU - Session URL Launcher Utility
===================================

**SULU** is a *proxy/dispatcher* tool for *URL protocol handlers*. A *URL protocol* is the part of the url before the colon (:). For example, *http* or *https* are common URL protocols.  Specifically, **SULU** is intended to handle protocols like *ssh*,  *rdp*, and *telnet* but it can be used for any legitimate protocol. 

**SULU** lets you register URL protocols on a computer, then configure installed applications to handle URLs of the registered protocol types. The configuration also supports definining variables that pull their values from the encoded URL which may be passed on the command line or via **SULU** after configuration of supported parameters to tools like *ssh, rdesktop, RdpClient, FreeRDP* etc.

Features
--------
1. Registers any desired URL protocols to **SULU**.
2. Assigns local applications to handle URL protocols. 
3. Allows supported session parameters values to be set and passed to assigned applications when passing on session URLs. 
4. Allows users to export **SULU** configuration files for backups and to expedite multiple installations. 
5. Allows users to import **SULU** configuration files for rapid installation. 

Example Usage Scenario
----------------------

One example of how **SULU** may be used is in the context of administration and usage of privileged sessions. [Safeguard for Priviliged Passwords](https://www.oneidentity.com/products/one-identity-safeguard-for-privileged-passwords/) (a product from  [One Identity](https://www.oneidentity.com/)) allows administrators to configure which applications on the computers of their end users will handle the URLs generated when launching priviliged sessions, and what specific parameters will pass to the specified program (like suppressing a desktop background or opening at a certain height and width).

Typically, administrators would associate **SULU** as the application to handle RDP, SSH and Telnet protocols on the computer, but this may also be done by anyone with sufficient privileges.

When **SULU** is opened, either by a web browser launching a privileged session or by manually running the executable, specific applications may be assigned to each URL protocol. 

Once **SULU**  has an application assigned for a given protocol, it will seamlessly handle session URLs behind the scene and pass on correctly formatted session requests including all supported parameters to the assigned application. 

The configured launcher’s settings may be imported or exported to expedite volume configuration for administrative purposes. 


## Getting Started

First, register **SULU** to handle the URL protocols you are interested in (*you'll need administrator priviliges for this step*):

```
sulu.exe register -p rdp ssh
```

Next, configure **SULU** to launch a *remote session application* for a protocol you wish to support.  The following command will launch the configuration UI in your browser. Note that all changes made using the UI are captured in the *sulu.json* configuration file.

```
sulu.exe ui
```
You may add one or more applications to the select list for each protocol type, but only one program is actively assigned at a given time. 

Once configured, when something (like your browser) launches a URL with one of the registered protocols, **SULU** will automatically trigger the configured application and transparently pass the incoming url protocol string to the assigned application along with any protocol- specific configured parameter values. 

## Testing
Test example in terminal:

```
explorer.exe rdp://123.45.6.7/test/url
```

Unregistering protocols:
```
sulu.exe unregister -p rdp ssh
```
## More Examples
__Configure Firefox to allow SSH protocol handlers:__

Navigate to about:config in Firefox. Copy and paste these values, and select the  *+* to add them as *BOOL* and make sure the value is set to *true*:
```
network.protocol-handler.external.ssh
network.protocol-handler.expose.ssh
```
Paste these values, select *+* to add them as BOOL and make sure the value is set to *false*:
```
network.protocol-handler.warn-external.ssh
```

__Manual registration on Ubuntu__

Run these commands from bash prompt:
```
cat << EOF > ~/.local/share/applications/sulu.desktop
[Desktop Entry]
Name=Sulu
Comment=Session URL Launcher Utility
Exec=/usr/bin/dotnet /<path to sulu>/Sulu.dll launch -u %u
Terminal=false
Type=Application
MimeType=x-scheme-handler/ssh;x-scheme-handler/rdp;x-scheme-handler/telnet
EOF

xdg-mime default sulu.desktop x-scheme-handler/rdp
xdg-mime default sulu.desktop x-scheme-handler/ssh
xdg-mime default sulu.desktop x-scheme-handler/telnet
```
__Installing on a MacOS machine (shown is the process to set up for use with Safeguard for Privileged Passwords)__
1. Install some helper utilities:
    Install *homebrew* then use it to install the needed software as follows:
    
    ```
    brew cask install firefox xquartz dotnet-sdk (xquartz requires a computer reboot before it can be used)
    brew install freerdp
    ```
        
    Download the app [*Platypus*](https://sveinbjorn.org/platypus)
    
    Download **SULU** 

2. Create a shell script:

   ```
   #!/bin/bash
    /usr/local/bin/dotnet <full_path_to_Sulu.dll> launch -u $1
    ```
    
3. Use Platypus to create an app from the shell script (see Platypus.png)

4. Log in to Safeguard with Firefox

5. Create some SSH and RDP requests if you don't have any

6. Click the launch/play button for an approved session request.
When Firefox asks how to open the link, pick "choose another application" and browse to the app you created with Platypus (Firefox should now remember it for subsequent uses).

__Troubleshooting:__
If things aren't working as expected, check the *sulu.log file*.

With [freerdp](http://www.freerdp.com/), some extra work may be required depending on how permissions are set up.

1. Let freerdp write to your user's config folder
        
       ```
      sudo chmod 777 ~/.config
       ```
       
If that's not enough, you may have to accept the certificate.
    
2. Open the sulu log and look for something like the following
        
         ```
         2020-10-22 12:10:03.816 -06:00 [DBG] Dispatching URL: rdp://full+address=s:10.5.32.168:3389&username=s:localhost%5cvaultaddress%7e10.5.33.238%25token%7etW4BZDEjhYne67yDvPcj4x6kvjYR2MDqE1cxKrMz8Mj8hrheCHRjs2jr8%25Account04%2510.5.52.79:3389
            2020-10-22 12:10:03.850 -06:00 [DBG] Starting external application: '/usr/local/bin/xfreerdp' with args: '/u:localhost\vaultaddress~10.5.33.238%token~tW4BZDEjhYne67yDvPcj4x6kvjYR2MDqE1cxKrMz8Mj8hrheCHRjs2jr8%Account04%10.5.52.79:3389 /v:10.5.32.168:3389 /p:Safeguard /cert:ignore'
        ```
            
3. Using that information, run the command yourself in the terminal, the external application and args. In this example it would be:
        
        ```
            /usr/local/bin/xfreerdp /u:localhost\vaultaddress~10.5.33.238%token~tW4BZDEjhYne67yDvPcj4x6kvjYR2MDqE1cxKrMz8Mj8hrheCHRjs2jr8%Account04%10.5.52.79:3389 /v:10.5.32.168:3389 /p:Safeguard /cert:ignore
            ```
            
4. If you're prompted about a certificate, choose T (not Y).
    

