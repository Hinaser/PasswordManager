# PasswordManager
A password management tool for Windows platform. The tool enables you to pickup id/password easily and store private data with security.

# Prerequisite
.NET Framework 3.5 or higher on Windows platform.  
(.NET Framework 3.5 is installed on Windows7 by default)  

# Feature
This app will provide you features as follows:  

* Basic
 - Add/Modify/Delete passwords and folders.
* Accessibility
 - Combination of tree/list style data structure.
 - Double click a password item to copy id/password into clipboard.
* Security
 - Validate password strength by strength score originally defined for this application.
 - Lock password information by a master password.
 - Password file is encrypted by AES crypt algorithm if master password is set.
* Portability
 - Application itself is seperated from encrypted password file. So you can import/export password data easily. You can also easily make backups by copying the file.

# Remark
This application does not use Windows registry.

# License
This software is licensed under Apache License 2.0.  
See detail in http://www.apache.org/licenses/LICENSE-2.0  
 or abstract version of the license terms below.

Copyright 2015 Yunoske

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
