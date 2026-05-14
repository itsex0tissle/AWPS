var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ProjectResource> awps_iot_server = builder.AddProject<Projects.AWPS_IoT_Server>("awps-iot-server").WithExternalHttpEndpoints();
IResourceBuilder<ProjectResource> awps_ui_web = builder.AddProject<Projects.AWPS_UI_Web>("awps-ui-web").WaitFor(awps_iot_server).WithExternalHttpEndpoints();
awps_ui_web.WithReference(awps_ui_web).WithReference(awps_iot_server);
builder.Build().Run();