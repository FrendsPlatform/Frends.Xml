- [Frends.Xml](#frends.xml)
   - [Installing](#installing)
   - [Building](#building)
   - [Contributing](#contributing)
   - [Documentation](#documentation)
     - [Xml.XpathQuery](#xml.xpathquery)
       - [Input](#input)
       - [Options](#options)
       - [Result](#result)
     - [Xml.XpathQuerySingle](#xml.xpathquerysingle)
       - [Input](#input)
       - [Options](#options)
       - [Result](#result)
     - [Xml.Transform](#xml.transform)
       - [Input](#input)
       - [Result](#result)
     - [Xml.Validate](#xml.validate)
       - [Input](#input)
       - [Options](#options)
       - [Result](#result)
     - [Xml.ConvertJsonToXml](#xml.convertjsontoxml)
       - [Input](#input)
       - [Result](#result)
   - [License](#license)
   
# Frends.Xml
FRENDS XML processing tasks.

## Installing
You can install the task via FRENDS UI Task view or you can find the nuget package from the following nuget feed
`https://www.myget.org/F/frends/api/v2`

## Building
Ensure that you have `https://www.myget.org/F/frends/api/v2` added to your nuget feeds

Clone a copy of the repo

`git clone https://github.com/FrendsPlatform/Frends.Xml.git`

Restore dependencies

`nuget restore frends.xml`

Rebuild the project

Run Tests with nunit3. Tests can be found under

`Frends.Xml.Tests\bin\Release\Frends.Xml.Tests.dll`

Create a nuget package

`nuget pack nuspec/Frends.Xml.nuspec`

## Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

## Documentation

### Xml.XpathQuery
Query XML with XPath and return a list of results.

#### Input

| Property        | Type     | Description       |
|-----------------|----------|-------------------|
| Xml             | string   | XML to be queried | 
| XpathQuery      | string   | XPath query       |

#### Options

| Property                 | Type             | Description                                    |
|--------------------------|------------------|------------------------------------------------|
| ThrowErrorOnEmptyResults | bool             | Task will throw an exception if no results found |
| XpathVersion             | Enum{V3, V2, V1} | Select the XPath version for the query |

#### Result

| Property/Method   | Type           | Description                 |
|-------------------|----------------|-----------------------------|
| Data              | List<object>   | List of query results. Object type depends on the query. If selecting a node the type will be string and contain the xml node.  |
| ToJson()          | List<JToken>   | Returns a Json representation of the xml data. It is possible to access this date by dot notation. `#result.ToJson()[0].Foo.Bar` |
| ToJson(int index) | JToken         | Returns a single result as Json  |


### Xml.XpathQuerySingle

Query XML with XPath and return a single result.

#### Input

| Property        | Type     | Description       |
|-----------------|----------|-------------------|
| Xml             | string   | XML to be queried | 
| XpathQuery      | string   | XPath query       |

#### Options

| Property                 | Type             | Description                                    |
|--------------------------|------------------|------------------------------------------------|
| ThrowErrorOnEmptyResults | bool             | Task will throw an exception if no results found |
| XpathVersion             | Enum{V3, V2, V1} | Select the XPath version for the query |

#### Result

| Property/Method   | Type           | Description                 |
|-------------------|----------------|-----------------------------|
| Data              | object         | Object type depends on the query. If selecting a node the type will be string and contain the xml node.  |
| ToJson()          | JToken         | Returns a Json representation of the xml data. It is possible to access this date by dot notation. `#result.ToJson().Foo.Bar` |

### Xml.Transform 

Create a XSLT transformation.

#### Input

| Property        | Type                             | Description                  |
|-----------------|----------------------------------|------------------------------|
| Xml             | string                           |                              | 
| Xslt            | string                           |                              | 
| XsltParameters  | List {string Name, string Value} |                              |

#### Result
string

### Xml.ConvertJsonToXml

This task takes JSON text and deserializes it into an xml text.
Because valid XML must have one root element, the JSON passed to the task should have one property in the root JSON object. If the root JSON object has multiple properties, then the XmlRootElementName should be used. A root element with that name will be inserted into the XML text.

Example input json: 
```json
@"{
  '?xml': {
    '@version': '1.0',
    '@standalone': 'no'
  },
  'root': {
    'person': [
      {
        '@id': '1',
        'name': 'Alan'
      },
      {
        '@id': '2',
        'name': 'Louis'
      }
    ]
  }
"
````

Example result:
```xml
<?xml version="1.0" standalone="no"?>
 <root>
   <person id="1">
     <name>Alan</name>
   </person>
   <person id="2">
     <name>Louis</name>
   </person>
</root>
```
#### Input

| Property        | Type      | Description                                  |
|-----------------|-----------|----------------------------------------------|
| Json             | string   | Json string to be converted to XML  |
| XmlRootElementName      | string  | The name for the root XML element |


#### Result

string

## License

This project is licensed under the MIT License - see the LICENSE file for details
