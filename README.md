### Background Job

### Project Structure

 - BackgroundJob.AppHost
    - Aspire project
 - BackgroundJob.Producer
    - application specific api
    - create entry into mssql queue for bg operation
 - BackgroundJob.Distributor
    - background worker service
    - look for sql queue tables
    - put entry into mongo collection
    - put entry into rabbitMQ
 - BackgroundJob.Worker
    - background worker service
    - take item from rabbitMQ for processing
    - with the help of services from BackgroundJob.Producer it will process the job & mark success/fail entry


### Job Structure
- Job
    - Tasks
        - Statuses

## Future Scope
- Improve logging
- Add serilog sinks
- improve EF core entity structure
- add all services to docker

 

