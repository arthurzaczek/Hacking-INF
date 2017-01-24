import { Component, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';

import { AppComponent } from './app.component';
import { HomeComponent } from './home.component';
import { InfoComponent } from './info.component';
import { MeComponent } from './me.component';
import { CoursesComponent } from './courses.component';
import { CourseDetailComponent } from './course-detail.component';
import { ExampleDetailComponent } from './example-detail.component';
import { LoginComponent } from './login.component';
import { AdminDownloadComponent } from './admin-download.component';

import { MarkdownComponent } from './markdown.component';
import { HackingService } from './hacking.service';
import { HttpClient } from './http-client';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'info', component: InfoComponent },
            { path: 'me', component: MeComponent },
            { path: 'login', component: LoginComponent },
            { path: 'courses', component: CoursesComponent },
            { path: 'course/:name', component: CourseDetailComponent },
            { path: 'example/:course/:name', component: ExampleDetailComponent },
            { path: 'admin-download', component: AdminDownloadComponent },
        ])
    ],
    declarations: [
        AppComponent,
        HomeComponent,
        InfoComponent,
        MeComponent,
        CoursesComponent,
        CourseDetailComponent,
        ExampleDetailComponent,
        LoginComponent,
        AdminDownloadComponent,
        MarkdownComponent,
    ],
    providers: [
        HackingService,
        HttpClient,
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
