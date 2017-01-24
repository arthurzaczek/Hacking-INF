import { Component, OnInit } from '@angular/core';
import { User } from './models';
import { HackingService } from './hacking.service';

import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Rx';

@Component({
    selector: 'my-app',
    templateUrl: 'app/app.component.html'
})
export class AppComponent implements OnInit {
    Year = new Date().getFullYear();
    User: User = <User>{};
    subscription: Subscription;

    constructor(private hackingService: HackingService) {
    }

    ngOnInit(): void {
        var self = this;
        this.subscription = this.hackingService
            .userLoginEvent
            .subscribe(item => {
                self.User = item;
            });
        this.hackingService.whoAmI();
        let timer = Observable.timer(3600 * 1000, 3600 * 1000);
        timer.subscribe(t => {
            this.hackingService.whoAmI();
        });
    }

    logout(event: any): void {
        if (event != null) event.preventDefault();

        this.hackingService.logout();
    }
}
