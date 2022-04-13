import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ChangeDetectionStrategy, NgZone } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import * as signalR from "@microsoft/signalr";
import * as faker from 'faker';

interface IFormValue {
    message: string;
}

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent implements OnInit {

    public form: FormGroup;
    // public canConnect: boolean = true;
    public connected: boolean;
    private token: string;
    private connection: signalR.HubConnection;
    private baseUrl = 'http://localhost:3101';
    public constructor(
        // private snackBar: MatSnackBar,
        private httpClient: HttpClient,
        fb: FormBuilder
    ) {
        this.form = fb.group({
            username: [],
            password: [],
            message: [faker.random.words()]
        });
    }

    public ngOnInit(): void {
        // let lastLoginStr = localStorage.getItem('latest_login');
        // if (lastLoginStr) {
        //     this.form.patchValue(JSON.parse(lastLoginStr));
        // }

        // this.connection = new signalR.HubConnectionBuilder()
        //     // .withUrl(`${this.baseUrl}/hub/chathub`, {
        //         .withUrl(`${this.baseUrl}/chathub`, {
        //         accessTokenFactory() {
        //             return localStorage.getItem('token');
        //         },
        //         transport: signalR.HttpTransportType.WebSockets
        //     })
        //     .withAutomaticReconnect()
        //     .build();

        // // this.connection.on("ReceiveMessage", (user, message) => {
        // //     // console.log('messageReceived',message,aa);
        // //     console.log(`用户:${user} 消息${message}`);
        // //     // this.snackBar.open(`接收到消息:${message}`, null, { duration: 2000 });
        // // });

        // // this.connect();
        // this.login();
    }

    // public async login(): Promise<void> {
    //     let url: string = `${this.baseUrl}/api/Token/Request`;
    //     this.httpClient.post<any>(url, { username: "admin", "password": "admin" }).subscribe(res => {
    //         this.token = res.token;
    //         // this.canConnect = true;
    //         localStorage.setItem('token', res.token);
    //         // localStorage.setItem('latest_login', JSON.stringify(account));
    //         // this.snackBar.open(`登陆成功`, null, { duration: 2000 });
    //         console.log('登陆成功');
    //     });
    // }

    // public connect(): void {
    //     if (this.connected) {
    //         console.log('已经连接,无需重复连接');
    //         return;
    //     }
    //     this.connection.start().then(() => {
    //         console.log('连接成功');
    //         this.connected = true;
    //     }).catch(err => {
    //         console.log('无法连接到服务器:', err);
    //         this.connected = false;
    //     });
    // }

    // public async disConnect(): Promise<void> {
    //     await this.connection.stop();
    //     console.log('主动断开连接');
    //     this.connected = false;
    // }

    // public sendMessage(): void {
    //     const value: IFormValue = this.form.value;
    //     this.connection.invoke("SendMessage", "leon", value.message)
    //         .then(() => {
    //             this.form.patchValue({ message: faker.random.words() });
    //         })
    //         .catch((err) => {
    //             return console.error(err.toString());
    //         });
    // }


}
