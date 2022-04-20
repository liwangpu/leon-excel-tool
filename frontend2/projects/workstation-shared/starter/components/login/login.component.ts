import { Component, OnInit, ChangeDetectionStrategy, Injector, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { debounceTime } from 'rxjs';
import { SubSink } from 'subsink';
import * as fromCore from 'workstation-core';

interface IFormValue {
    username?: string;
    password?: string;
}
@Component({
    selector: 'workstation-shared-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent implements OnInit, OnDestroy {

    public errorMessage: string;
    public form: FormGroup;

    @fromCore.LazyService(FormBuilder)
    private readonly fb: FormBuilder;
    @fromCore.LazyService(ChangeDetectorRef)
    private readonly cd: ChangeDetectorRef;
    @fromCore.LazyService(ActivatedRoute)
    private readonly acr: ActivatedRoute;
    @fromCore.LazyService(Router)
    private readonly router: Router;
    @fromCore.LazyService(fromCore.IDENTITY_STORE)
    private readonly identityStore: fromCore.IdentityStore;
    private returnUrl: string;
    private messageHandler: any;
    private readonly subs: SubSink = new SubSink();
    public constructor(
        protected injector: Injector
    ) {
        this.form = this.fb.group({
            username: ['leon', [Validators.required]],
            password: ['123', [Validators.required]]
        });
    }
    public ngOnDestroy(): void {
        this.subs.unsubscribe();
    }

    public ngOnInit(): void {
        this.subs.sink = this.acr.queryParams.subscribe((params: { [key: string]: any }) => {
            const returnUrl: string = params.return ? params.return : null;
            this.returnUrl = returnUrl ? decodeURIComponent(returnUrl) : '/';
        });

        this.subs.sink = this.form.valueChanges
            .pipe(debounceTime(120))
            .subscribe(() => this.clearMessage());
    }

    public login(): void {
        const { username, password } = this.form.value as IFormValue;
        this.identityStore.login(username, password)
            .subscribe(async () => {
                await this.router.navigateByUrl(this.returnUrl);
            }, err => {
                this.showMessage('账户名或者密码有误');
            });
    }

    private showMessage(message: string): void {
        if (this.messageHandler) {
            clearTimeout(this.messageHandler);
            this.messageHandler = null;
        }
        this.errorMessage = message;
        this.cd.markForCheck();
        this.messageHandler = setTimeout(() => {
            this.errorMessage = null;
            this.cd.markForCheck();
        }, 3000);
    }

    private clearMessage(): void {
        if (this.messageHandler) {
            clearTimeout(this.messageHandler);
            this.messageHandler = null;
        }
        if (!this.errorMessage) { return; }
        this.errorMessage = null;
        this.cd.markForCheck();
    }

}
