import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { User } from './models';
import { HackingService } from './hacking.service';

import 'rxjs/add/operator/switchMap';

declare var jsHelper: any;

@Component({
    selector: 'course-detail',
    templateUrl: 'app/login.component.html'
})
export class LoginComponent implements OnInit {
    constructor(
        private hackingService: HackingService,
        private router: Router,
        private location: Location) { }

    user: User = <User>{};
    invalidLogin: boolean = false;

    ngOnInit(): void {

    }

    login(): void {
        jsHelper.showWaitDialog();
        var self = this;
        this.hackingService
            .login(this.user)
            .subscribe(data => {
                jsHelper.hideWaitDialog();
                self.user = data;
                self.location.back();
            }, error => {
                jsHelper.hideWaitDialog();
                self.invalidLogin = true;
            });
    }
}