# Alternative Approach Analysis for CFC Ticket Watcher

## Current Proposed Architecture

```
GetTicketInformation (Timer Trigger) -> Blob Storage
ProcessTicketUpdate (Blob Trigger) -> Table Storage  
ProcessUpcomingMatches (Timer Trigger + Table Storage) -> Queue Storage
SendEmail (Queue Trigger) -> Azure Communication Services
```

## Analysis of Current Approach

### Strengths
1. **Decoupled Architecture**: Each function has a single responsibility
2. **Durable Storage**: Blob storage provides a history of API responses
3. **Table Storage**: Efficient for querying upcoming matches by partition/row key
4. **Queue-based Email**: Provides retry capability and rate limiting for emails

### Potential Issues
1. **Polling Trigger**: The term "Polling Trigger" in Azure Functions typically means a Timer Trigger. However, there's no native "Polling Trigger" binding.
2. **Table Storage Trigger**: Azure Functions does not have a native Table Storage trigger. You would need to use a Timer trigger to poll the table, or use Azure Event Grid.
3. **ProcessUpcomingMatches Logic**: Comparing "what matches are upcoming" requires state management to track what notifications have been sent.

## Recommended Alternative Architecture

### Option 1: Timer + Blob + Event Grid (Recommended)

```
GetTicketInformation (Timer Trigger @"0 */15 * * * *") 
    -> Writes to Blob Storage
    -> Event Grid automatically fires on blob creation

ProcessTicketUpdate (Event Grid Trigger on Blob Created)
    -> Reads blob, extracts matches
    -> Writes to Table Storage
    -> Publishes matches with tickets available to Queue

ProcessUpcomingMatches (Queue Trigger)
    -> Compares against "SentNotifications" table
    -> If new match with tickets, writes to Email Queue

SendEmail (Queue Trigger)
    -> Sends email via Azure Communication Services
```

### Option 2: Durable Functions (For Complex Orchestration)

If you need more complex orchestration, retry logic, or long-running processes:

```csharp
[FunctionName("TicketWatcherOrchestrator")]
public static async Task RunOrchestrator(
    [OrchestrationTrigger] IDurableOrchestrationContext context)
{
    var ticketData = await context.CallActivityAsync<TicketResponse>("FetchTicketData");
    var matches = await context.CallActivityAsync<List<Match>>("ParseMatches", ticketData);
    var newMatches = await context.CallActivityAsync<List<Match>>("FilterNewMatches", matches);
    
    foreach (var match in newMatches)
    {
        await context.CallActivityAsync("SendNotification", match);
    }
}
```

### Option 3: Simplified Two-Function Approach

If simplicity is preferred:

```
CheckForTickets (Timer Trigger @"0 */15 * * * *")
    -> Fetches API
    -> Compares with Table Storage
    -> Sends email for new matches
    -> Updates Table Storage

This reduces complexity but loses the audit trail of raw API responses.
```

## Implementation Decision

Given the requirements, I'm implementing a hybrid approach:

1. **GetTicketInformation**: Timer Trigger (runs every 15 minutes) -> Blob Storage output
2. **ProcessTicketUpdate**: Blob Trigger -> Parses data, stores in Table Storage, queues new matches
3. **ProcessUpcomingMatches**: Queue Trigger -> Checks if notification was already sent, if not, queues email
4. **SendEmail**: Queue Trigger -> Azure Communication Services

Key changes from original proposal:
- **No "Polling Trigger"**: Using Timer Trigger for GetTicketInformation
- **No "Table Storage Trigger"**: Removed ProcessUpcomingMatches as Timer+Table. Instead, ProcessTicketUpdate directly queues new matches found
- **Added notification tracking**: Table to track sent notifications and avoid duplicates

## Storage Schema

### Blob Storage
- Container: `ticket-data`
- Path: `responses/{yyyy}/{MM}/{dd}/{HH}-{mm}-{ss}.json`

### Table Storage
- **UpcomingMatches Table**
  - PartitionKey: Season (e.g., "2025")
  - RowKey: MatchID (e.g., "g2563072")
  - Properties: MatchLabel, MatchDate, Competition, HomeTeam, AwayTeam, TicketsAvailable, LastUpdated

- **SentNotifications Table**
  - PartitionKey: MatchID
  - RowKey: NotificationType (e.g., "TicketAvailable")
  - Properties: SentAt, EmailTo

### Queue Storage
- **new-matches-queue**: For matches that need notification checking
- **email-queue**: For emails to be sent

## Configuration Required

```json
{
  "AzureWebJobsStorage": "UseDevelopmentStorage=true",
  "TicketApiUrl": "https://webapi.gc.celticfc.com/v1/pages/byfullpath?fullPath=tickets",
  "AzureCommunicationServicesConnectionString": "<your-acs-connection-string>",
  "NotificationEmailFrom": "tickets@yourdomain.com",
  "NotificationEmailTo": "subscriber@example.com"
}
```
