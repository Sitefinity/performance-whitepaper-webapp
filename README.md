 ### **Requirements**

- Sitefinity CMS license
- .NET Framework 4.5
- Visual Studio 2012 or newer versions
- Microsoft SQL Server 2014 or newer versions
- Windows Identity Foundation

  **NOTE:** Depending on the Microsoft OS version you are working with, the method for downloading and installing, or for  enabling the   identity framework differs:
  - **Windows 7**  - download from  [Windows Identity Foundation](http://www.microsoft.com/en-us/download/details.aspx?id=17331)
  - **Windows 8**  - in the Control Panel, turn on the relevant Windows feature *Windows Identity Foundation 3.5*

For a complete list of the solution requirements, see [System requirements](http://docs.sitefinity.com/system-requirements).

### **Prerequisites**

<h4>Clear the NuGet cache files</h4>

1. In Windows Explorer, open the  **%localappdata%\NuGet\Cache**  folder.
2. Select all files and delete them.

<h4>Restore the database backup file to your SQL Server</h4>

1. Download the database from the following [blob storage](https://sitefinitycistorage.blob.core.windows.net/performance-whitepaper/Sitefinity_10.1.6500.0.zip) file.
2. Unzip the downloaded **.zip** file.
3. In SQL Management Studio, open the context menu of _Databases_ and click _Restore Database..._
4. Navigate to the folder where you unzipped the downloaded **.zip** file.
5. Select the **Sitefinity.bak** file.
6. Click _OK_.

<h4>Powershell Execution Policy</h4>

Make sure to set the appropriate **Powershell** execution policy, so that you avoid build errors. To do this, open your _Visual Studio_&#39;s _Package Manager Console_ and execute the following command:
**Set-ExecutionPolicy RemoteSigned**

<h4>Run the project in Load balancing environment</h4>

1. Configure Redis cache.
In Sitefinity CMS backend, navigate to *Administration* » *Settings* » *Advanced* » *System* » *LoadBalancing* » *Redis Settings* » *Connection String*.
2. Set the **machineKey** element in your **web.config** file.
3. Deploy the same instance and configurations on all web nodes.

<h4>Use website personalization</h4>

Personalization database files are excluded from the sample project because of license limitations. You can obtain personalization database files under the **~/SitefinityWebApp/App_Data/GeoLocation** folder of an existing installation of Sitefinity CMS.

### **Nuget package restoration**

The solution in this repository relies on NuGet packages with Nuget Package Restore enabled.
For detailed instructions for adding the Sitefinity Nuget server, go to [Sitefinity NuGet repository](http://nuget.sitefinity.com/#/home).

For a full list of the referenced packages and their versions see the [**packages.config**](https://github.com/Sitefinity/performance-whitepaper-webapp/blob/master/packages.config).
For a history and additional information related to package versions on different releases of this repository, see the page [*Releases*](https://github.com/Sitefinity/performance-whitepaper-webapp/releases).

### **Installation instructions:**

1. In Solution Explorer, navigate to _SitefinityWebApp_ -&gt; _App\_Data_ -&gt; _Sitefinity_ -&gt; _Configuration_ and select the  **DataConfig.config**  file.
2. Modify the **connectionString** values to match your server address.
3. Build the solution.

### **Login**

To log in Sitefinity CMS backend, use the following credentials:

**Username:**  admin  
**Password:**  password

### **Disclamer**

The sample code is provided on an "AS IS" basis for preview and testing purposes. Oners makes no warranties, expressed or implied, and disclaims all implied warranties including, without limitation, the implied warranties of merchantability or of fitness for a particular purpose. The entire risk arising out of the use or performance of the sample code is borne by the user. 
