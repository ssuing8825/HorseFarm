To create message queues, open the powershell console and execute the following commands with the correct path for the 
included powershell script:

<path>\CreateMsmqQueue.ps1 -c "<relative path>/Staging.svc" Y Everyone all T

NOTE: You must replace the <relative path> with the exact relative path in IIS of the target .svc file.