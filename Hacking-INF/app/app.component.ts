import { Component, OnInit, Compiler} from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { User } from './models';
import { HackingService } from './hacking.service';

import { Subscription, Observable, timer } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
    selector: 'my-app',
    templateUrl: 'app.component.html'
})
export class AppComponent implements OnInit {
    Year = new Date().getFullYear();
    User: User = <User>{};
    subscription: Subscription;

    containerClass: string = "container";

    constructor(private hackingService: HackingService,
        private compiler: Compiler,
        private router: Router) {
        router.events.pipe(
                filter((event: any) => event instanceof NavigationEnd)
            ).subscribe((val: NavigationEnd) => {
                if (val.url.startsWith("/example/")) {
                    this.containerClass = "container-fluid";
                } else {
                    this.containerClass = "container";
                }
            });
    }

    ngOnInit(): void {
        var self = this;
        this.subscription = this.hackingService
            .userLoginEvent
            .subscribe(item => {
                console.log("User changed, update UI");
                self.User = item;
            });
        this.hackingService.whoAmI();
        let t = timer(3600 * 1000, 3600 * 1000);
        t.subscribe(() => {
            this.hackingService.whoAmI();
        });
    }

    logout(event: any): void {
        if (event != null) event.preventDefault();

        this.hackingService.logout();
    }
}
