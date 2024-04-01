# File Upload Service (FUS)

The File Upload Service (FUS) is a service that allows users to upload and 
download files to the server. The service is implemented as a RESTful API and 
is available at the following endpoints:

```http request
POST /api/files/upload
Request Headers:
  Content-Type: multipart/form-data
Request Body:
    file: The file to upload
Response:
    200 OK: The file was uploaded successfully
    400 Bad Request: The request was invalid
    500 Internal Server Error: An error occurred on the server
```

```http request
GET /api/files/download/{id}
Request Headers:
  Accept: application/octet-stream
Request Parameters:
    id: The ID of the file to download
Response body:
    The file to download
Response:
    200 OK: The file was downloaded successfully
    400 Bad Request: The request was invalid
    404 Not Found: The file was not found
    500 Internal Server Error: An error occurred on the server
```

## EFCore

### AOT Model

The AOT model is a model that is generated at compile time using the EFCore 
dotnet cli tool

```bash
dotnet ef dbcontext optimize --output-dir EFCompiledModels --namespace DTA.FUS.EFCompiledModels --configuration Debug-JIT -v
```

curl -X 'POST' \
'http://localhost:5224/api/file/upload' \
-H 'accept: */*' \
-H 'Content-Type: multipart/form-data' \
-F 'file=@Logo.svg;type=image/svg+xml'