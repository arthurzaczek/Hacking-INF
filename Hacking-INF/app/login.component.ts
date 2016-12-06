import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from './models';
import { HackingService } from './hacking.service';

import 'rxjs/add/operator/switchMap';

@Component({
    selector: 'course-detail',
    templateUrl: 'app/login.component.html'
})
export class LoginComponent implements OnInit {
    constructor(
        private hackingService: HackingService,
        private router: Router) { }

    user: User = <User>{};
    invalidLogin: boolean = false;

    ngOnInit(): void {

    }

    login(): void {
        var self = this;
        this.hackingService
            .login(this.user)
            .subscribe(data => {
                self.user = data;
                self.router.navigate(['/']);
            }, error => {
                self.invalidLogin = true;
            });
    }
}