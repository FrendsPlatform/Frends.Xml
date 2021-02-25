- [Frends.Xml](#frends.xml)
   - [Installing](#installing)
   - [Building](#building)
   - [Contributing](#contributing)
   - [Documentation](#documentation)
     - [Xml.XpathQuery](#xmlxpathquery) 
     - [Xml.XpathQuerySingle](#xmlxpathquerysingle)
     - [Xml.Transform](#xmltransform)
     - [Xml.Validate](#xmlvalidate)
     - [Xml.ConvertJsonToXml](#xmlconvertjsontoxml)
   - [License](#license)
   
# Frends.Xml
FRENDS XML processing tasks.

## Installing
You can install the task via FRENDS UI Task view, by searching for packages. You can also download the latest NuGet package from https://www.myget.org/feed/frends/package/nuget/Frends.Xml and import it manually via the Task view.

## Building
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

### Xml.Validate

Validate XML against XML Schema Definitions.

#### Input

| Property        | Type     | Description                      |
|-----------------|----------|----------------------------------|
| Xml             | string   | XML to validate                  | 
| XsdSchemas      | string[] | List of XML Schema Definitions   |

Example input Xml: 
```xml
<?xml version="1.0"?>
<catalog>
  <book id="book1">
    <author>My favorite author</author>
    <title>XML Developer’s Guide</title>
    <genre>Markup</genre>
    <price>44.95</price>
    <publish_date>2002-09-24</publish_date>
    <description>An in-depth look at creating applications with XML.</description>
  </book>
</catalog>
````

Example input XSD schema:
```xml
<?xml version="1.0"?>
<xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
<xs:element name="catalog" type="catalogType"/>
  <xs:complexType name="bookType">
    <xs:sequence>
      <xs:element type="xs:string" name="author"/>
      <xs:element type="xs:string" name="title"/>
      <xs:element type="xs:string" name="genre"/>
      <xs:element type="xs:float" name="price"/>
      <xs:element type="xs:date" name="publish_date"/>
      <xs:element type="xs:string" name="description"/>
      </xs:sequence>
        <xs:attribute type="xs:string" name="id"/>
      </xs:complexType>
      <xs:complexType name="catalogType">
    <xs:sequence>
      <xs:element type="bookType" name="book"/>
    </xs:sequence>
  </xs:complexType>
</xs:schema>
```

#### Failing example

With the same XSD schema above, this XML:
```xml
<?xml version="1.0"?>
<catalog>
  <book id="book1">
    <author>My favorite author</author>
    <title>XML Developer’s Guide</title>
    <genre>Markup</genre>
    <price>44.95</price>
    <publish_date>2002-09-24-9:00</publish_date>
    <description>An in-depth look at creating applications with XML.</description>
  </book>
</catalog>
````

would return:

```
IsValid: false
Error: The 'publish_date' element is invalid - The value '2002-09-24-9:00' is invalid according to its datatype 'http://www.w3.org/2001/XMLSchema:date' - The string '2002-09-24-9:00' is not a valid Date value.
``` 

#### Options

| Property                 | Type             | Description                          |
|--------------------------|------------------|--------------------------------------|
| ThrowOnValidationErrors  | bool             | Throw exception on validation error. |

#### Result

| Property   | Type   | Description              |
|------------|--------|--------------------------|
| IsValid    | bool   | Is the Xml valid or not  |
| Error      | string | Validation error message |

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
