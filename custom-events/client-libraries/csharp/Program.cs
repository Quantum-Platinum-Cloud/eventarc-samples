﻿// Copyright 2022 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// [START eventarc_custom_publish_csharp]
using Google.Cloud.Eventarc.Publishing.V1;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
//using CloudNative.CloudEvents.Protobuf;

var commandArgs = Environment.GetCommandLineArgs();
var ProjectId = commandArgs[1]; // "events-atamel";
var Region = commandArgs[2];    // "us-central1";
var Channel = commandArgs[3];   // "hello-custom-events-channel";
Console.WriteLine($"ProjectId: {ProjectId}, Region: {Region}, Channel: {Channel}");

var publisherClient = await PublisherClient.CreateAsync();

//Construct the CloudEvent and set necessary attributes.
var cloudEventAttributes = new[]
{
    CloudEventAttribute.CreateExtension("someattribute", CloudEventAttributeType.String),
    CloudEventAttribute.CreateExtension("temperature", CloudEventAttributeType.Integer),
    CloudEventAttribute.CreateExtension("weather", CloudEventAttributeType.String),
};

var cloudEvent = new CloudEvent(cloudEventAttributes)
{
    Id = "12345",
    // Note: Type has to match with the trigger!
    Type = "mycompany.myorg.myproject.v1.myevent",
    Source = new Uri("urn:csharp/client/library"),
    Subject = "test-event-subject",
    DataContentType = "application/json",
    Data = "{\"message\": \"Hello World from latest C# client library\"}",
    Time = DateTimeOffset.UtcNow,
    // Note: someattribute and somevalue have to match with the trigger!
    ["someattribute"] = "somevalue",
    ["temperature"] = 5,
    ["weather"] = "sunny"
};

// Convert the CloudEvent to proto
// var cloudEventProto = new ProtobufEventFormatter().ConvertToProto(cloudEvent);

// Convert the CloudEvent to JSON
var formatter = new JsonEventFormatter();
var cloudEventJson = formatter.ConvertToJsonElement(cloudEvent).ToString();
Console.WriteLine($"Sending CloudEvent: {cloudEventJson}");

var request = new PublishEventsRequest
{
    Channel = $"projects/{ProjectId}/locations/{Region}/channels/{Channel}",
    // Events = { Any.Pack(cloudEventProto) },
    TextEvents = { cloudEventJson }
};
var response = await publisherClient.PublishEventsAsync(request);
Console.WriteLine("Event published!");
// [END eventarc_custom_publish_csharp]
