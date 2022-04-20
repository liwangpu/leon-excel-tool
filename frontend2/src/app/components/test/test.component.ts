import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { io, Socket } from 'socket.io-client';
import * as faker from 'faker';

@Component({
  selector: 'app-test',
  templateUrl: './test.component.html',
  styleUrls: ['./test.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TestComponent implements OnInit {

    public socket: Socket;
    public constructor() {
  
    }
  
    public ngOnInit(): void {
    //   this.generateMessage();
      this.socket = io('ws://localhost:3000', { transports: ["websocket"] });
      this.socket.on('connect', () => {
        console.log('服务器连接成功!', this.socket.id);
  
        this.socket.on('chat message', msg => {
          console.log('mmm:', msg);
        //   this.messages.push(msg);
        });
      });
    }

}
