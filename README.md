# PasswordManager
This project creates password management tool for Windows platform. The tool enables you to pickup id/password easily and store private data with security.

# Pre-Requisition
.NET Framework 3.5 or higher on Windows platform.  
(.NET Framework 3.5 is installed Windows7 by default)  

# Feature
This app will provide you features as follows:  

* Basic
 - Add/Modify/Delete passwords
 - Passwords can be stored in custom folder tree structure
 - Save passwords to encrypted file
* Accessibility
 - Look around your private password easily by a combination of tree/list style data structure
 - Copy id/password to clipboard by double-click for easy pasting to other input form
 - Manage Folder/Sub folder for password container. Move password to any folder you want.
* Security
 - Save those password data into private file encrypted by AES crypt algorithm.
 - Validate password strength by strength score originally defined for this application.
* Portability
 - Application itself is seperated to encrypted password file. So you can import/export password data easily. You can also easily make backups by copying the file.

# Remark
When you start this app it will prompt you a master password, which is the most important and must-be-private string to protect your secret.  
So please pay much attention to choose the password string so as not to be expected easily by other people.

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
