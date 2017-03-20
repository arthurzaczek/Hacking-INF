import { Component, OnInit, Compiler} from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
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

    containerClass: string = "container";

    constructor(private hackingService: HackingService,
        private compiler: Compiler,
        private router: Router) {
        router.events
            .filter((event: any) => event instanceof NavigationEnd)
            .subscribe((val: NavigationEnd) => {
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
                self.User = item;
            });
        this.hackingService.whoAmI();
        let timer = Observable.timer(3600 * 1000, 3600 * 1000);
        timer.subscribe(t => {
            this.hackingService.whoAmI();
        });
    }

    clearCache(): void {
        this.compiler.clearCache();
    }

    logout(event: any): void {
        if (event != null) event.preventDefault();

        this.hackingService.logout();
    }


}
