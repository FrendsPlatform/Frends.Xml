[TOC]

# Task documentation #

## Xml.XpathQuery ##
Query XML with XPath and return a list of results.

### Input ###

| Property        | Type     | Description       |
|-----------------|----------|-------------------|
| Xml             | string   | XML to be queried | 
| XpathQuery      | string   | XPath query       |

### Options ###

| Property                 | Type             | Description                                    |
|--------------------------|------------------|------------------------------------------------|
| ThrowErrorOnEmptyResults | bool             | Task will throw an exception if no results found |
| XpathVersion             | Enum{V3, V2, V1} | Select the XPath version for the query |

### Result ###
object

| Property/Method   | Type           | Description                 |
|-------------------|----------------|-----------------------------|
| Data              | List<object>   | List of query results. Object type depends on the query. If selecting a node the type will be string and contain the xml node.  |
| ToJson()          | List<JToken>   | Returns a Json representation of the xml data. It is possible to access this date by dot notation. `#result.ToJson()[0].Foo.Bar` |
| ToJson(int index) | JToken         | Returns a single result as Json  |


## Xml.XpathQuerySingle ##

Query XML with XPath and return a single result.

### Input ###

| Property        | Type     | Description       |
|-----------------|----------|-------------------|
| Xml             | string   | XML to be queried | 
| XpathQuery      | string   | XPath query       |

### Options ###

| Property                 | Type             | Description                                    |
|--------------------------|------------------|------------------------------------------------|
| ThrowErrorOnEmptyResults | bool             | Task will throw an exception if no results found |
| XpathVersion             | Enum{V3, V2, V1} | Select the XPath version for the query |

### Result ###
object

| Property/Method   | Type           | Description                 |
|-------------------|----------------|-----------------------------|
| Data              | object         | Object type depends on the query. If selecting a node the type will be string and contain the xml node.  |
| ToJson()          | JToken         | Returns a Json representation of the xml data. It is possible to access this date by dot notation. `#result.ToJson().Foo.Bar` |

## Xml.Transform ##

Create a XSLT transformation.

### Input ###

| Property        | Type                             | Description                  |
|-----------------|----------------------------------|------------------------------|
| Xml             | string                           |                              | 
| Xslt            | string                           |                              | 
| XsltParameters  | List {string Name, string Value} |                              |

### Result ###
string

## Xml.Validate ##

Validate XML against XML Schema Definitions

### Input ###

| Property        | Type      | Description                                  |
|-----------------|-----------|----------------------------------------------|
| Xml             | dynamic   | Input must be of type string or XmlDocument  |
| XsdSchemas      | string[]  | List of XML Schema Definitions  |

### Options ###

| Property                | Type           | Description                                       |
|-------------------------|----------------|---------------------------------------------------|
| ThrowOnValidationErrors | bool           |                                                   | 

### Result ###
object

| Property          | Type     | Description                       |
|-------------------|----------|-----------------------------------|
| IsValid           | bool     | Indicates if xml is valid or not. |
| Error             | string   | If IsValid is false this field contains the error message otherwise it is empty |