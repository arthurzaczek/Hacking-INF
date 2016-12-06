import { Component, OnInit } from '@angular/core';
import { User } from './models';
import { HackingService } from './hacking.service';

import { Subscription } from 'rxjs/Subscription';

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
    }
}
