### **Requirements**

- Sitefinity CMS license
- .NET Framework 4.5
- Visual Studio 2012 or later versions
- Microsoft SQL Server 2014 or later versions
- Windows Identity Foundation NOTE: Depending on the Microsoft OS version you are using, the method for downloading and installing or enabling the identity framework differs:
  - **Windows 7**  - download from  Windows Identity Foundation : http://www.microsoft.com/en-us/download/details.aspx?id=17331
  - **Windows 8**  - in the Control Panel, turn on the relevant Windows feature Windows Identity Foundation 3.5

See a complete list of the system requirements for the solution here: http://docs.sitefinity.com/system-requirements.

### **Prerequisites**

Clear the NuGet cache files. To do this:

1. In Windows Explorer, open the  **%localappdata%\NuGet\Cache**  folder.
2. Select all files and delete them.

You need to restore the database backup file to your SQL Server. To do this:

1. Download the database from the following [blob storage](https://sitefinitycistorage.blob.core.windows.net/performance-whitepaper/Sitefinity_10.1.6500.0.zip).
2. Unzip the downloaded **.zip** file.
3. In SQL Management Studio, open the context menu of _Databases_ and click _Restore Database..._
4. Navigate to the folder where you unzipped the downloaded **.zip** file.
5. Select the **Sitefinity.bak** file.
6. Click _OK_.

Powershell Execution Policy

Please make sure to set the appropriate  **Powershell**  execution policy in order to avoid build errors. To do this open your _Visual Studio_&#39;s _Package Manager Console_ and execute the following command:
Set-ExecutionPolicy RemoteSigned

Running project in Load balancing 

1. You need to configure redis cache. In Sitefinity CMS backend, navigate to Administration » Settings » Advanced » System » LoadBalancing » Redis Settings » Connection String.
2. You need to set machineKey element in your web.config
3. You need to deploy the same instance and configurations on all web nodes.

### **Nuget package restoration**

The solution in this repository relies on NuGet packages with Nuget Package Restore enabled. Sitefinity&#39;s Nuget Repository and the instructions on how to add the Sitefinity NuGet server are available here : http://nuget.sitefinity.com/#/home.

For a full list of the referenced packages and their versions see the packages.config: https://github.com/Sitefinity/performance-whitepaper-webapp/blob/master/packages.config.
For a history and additional information related to package versions on different releases of this repository, see the Releases page: https://github.com/Sitefinity/performance-whitepaper-webapp/releases.

### **Installation instructions:**

1. In Solution Explorer, navigate to _SitefinityWebApp_ -&gt; _App\_Data_ -&gt; _Sitefinity_ -&gt; _Configuration_ and select the  **DataConfig.config**  file.
2. Modify the  **connectionString**  values to match your server address.
3. Build the solution.

### **Login**

To login to the Sitefinity CMS backend, use the following credentials:

**Username:**  admin  **Password:**  password

### **Disclamer**

The sample code is provided on an "AS IS" basis for preview and testing purposes. Oners makes no warranties, express or implied, and disclaims all implied warranties including, without limitation, the implied warranties of merchantability or of fitness for a particular purpose. The entire risk arising out of the use or performance of the sample code is borne by the user. 
