# DynamicImageResizer

An API Gateway Proxy Integration Lambda where it resizes images to given dimensions. The images
are specified to Lambda by it's name and Lambda finds them in S3. Also Lamda uses a cache in S3 where it
skips resizing same image to same dimensions.

You must include the below query parameters:
image-name: The name of the image. For example "lenna.jpeg"
width: The new width of the image.
height The new height of the image.

## Here are some steps to follow to get started from the command line:

Once you have edited your template and code you can deploy your application using the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) from the command line.

Install Amazon.Lambda.Tools Global Tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

Deploy function to AWS Lambda
```
    cd "BlueprintBaseName/src/BlueprintBaseName"
    dotnet lambda deploy-function
```
