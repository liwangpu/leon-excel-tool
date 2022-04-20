import { SocketModule } from '@app/socket';
import { Module } from '@nestjs/common';
import { EventGateway } from './event.gateway';

@Module({
    imports: [SocketModule],
    providers: [EventGateway]
})
export class EventModule { }
