# ServiceBusMonitor
A simple Azure Function running every five minutes to monitor Azure Service Bus queues, send e-mails using PostMark when configurated rules for the deadletter queues raise alarms and update an Elasticsearch index with data.

## Settings in Azure
```
[
  {
    "name": "AlertRules:FiveMinutePercentageThreshold",
    "value": "50",
    "slotSetting": false
  },
  {
    "name": "AlertRules:HighThreshold",
    "value": "100",
    "slotSetting": false
  },
  {
    "name": "AlertRules:LowThreshold",
    "value": "2",
    "slotSetting": false
  },
  {
    "name": "AlertRules:MediumThreshold",
    "value": "10",
    "slotSetting": false
  },
  {
    "name": "APPINSIGHTS_INSTRUMENTATIONKEY",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "AzureWebJobsStorage",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "ElasticConnection",
    "value": "https://{username}:{password}.{elasticsearch-instance}:9243",
    "slotSetting": false
  },
  {
    "name": "ElasticIndex",
    "value": "example-queue",
    "slotSetting": false
  },
  {
    "name": "EmailConfig:Recipient",
    "value": "where.to.send.the.alarm@example.com",
    "slotSetting": false
  },
  {
    "name": "EmailConfig:Sender",
    "value": "valid.postmark.sender@example.com",
    "slotSetting": false
  },
  {
    "name": "Environment",
    "value": "user for logging",
    "slotSetting": false
  },
  {
    "name": "FUNCTIONS_EXTENSION_VERSION",
    "value": "~2",
    "slotSetting": false
  },
  {
    "name": "FUNCTIONS_WORKER_RUNTIME",
    "value": "dotnet",
    "slotSetting": false
  },
  {
    "name": "LogQueue",
    "value": "elastic-log-index",
    "slotSetting": false
  },
  {
    "name": "PostMarkWarningTemplateId",
    "value": "15046353",
    "slotSetting": false
  },
  {
    "name": "ServiceBusConnection",
    "value": "Endpoint=sb://my-servicebux.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={key}",
    "slotSetting": false
  },
  {
    "name": "WEBSITE_CONTENTAZUREFILECONNECTIONSTRING",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "WEBSITE_CONTENTSHARE",
    "value": "",
    "slotSetting": false
  },
  {
    "name": "WEBSITE_RUN_FROM_PACKAGE",
    "value": "",
    "slotSetting": false
  }
]
```
