Command for creating table in dynamoDb
```
CreateTable
{
  "TableName": "Users",
  "AttributeDefinitions": [
    {
      "AttributeName": "pk",
      "AttributeType": "S"
    },
    {
      "AttributeName": "sk",
      "AttributeType": "S"
    }
  ],
  "KeySchema": [
    {
      "AttributeName": "pk",
      "KeyType": "HASH"
    },
    {
      "AttributeName": "sk",
      "KeyType": "RANGE"
    }
  ],
  "ProvisionedThroughput": {
    "ReadCapacityUnits": 2,
    "WriteCapacityUnits": 2
  }
}
```