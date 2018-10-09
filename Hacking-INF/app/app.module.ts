import { Component, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { HomeComponent } from './home.component';
import { InfoComponent } from './info.component';
import { MeComponent } from './me.component';
import { CoursesComponent } from './courses.component';
import { CourseDetailComponent } from './course-detail.component';
import { ExampleDetailComponent } from './example-detail.component';
import { LoginComponent } from './login.component';
import { AdminDownloadComponent } from './admin-download.component';
import { AdminExamplesComponent } from './admin-examples.component';
import { AdminStatsComponent } from './admin-stats.component';
import { AdminStatsStudentsComponent } from './admin-stats-students.component';
import { AdminLogfilesComponent } from './admin-logfiles.component';
import { AdminCompilerMessagesComponent } from './admin-compiler-messages.component';

import { MarkdownComponent } from './markdown.component';
import { HackingService } from './hacking.service';
import { HackingHttpClient } from './hacking-http-client';
import { AdminLoadTestComponent } from "./admin-loadtest.component";

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpClientModule,
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
            { path: 'admin-examples', component: AdminExamplesComponent },
            { path: 'admin-loadtest', component: AdminLoadTestComponent },
            { path: 'admin-stats', component: AdminStatsComponent },
            { path: 'admin-stats-students', component: AdminStatsStudentsComponent },
            { path: 'admin-logfiles/:type', component: AdminLogfilesComponent },
            { path: 'admin-compiler-messages', component: AdminCompilerMessagesComponent },
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
        AdminExamplesComponent,
        AdminStatsComponent,
        AdminStatsStudentsComponent,
        AdminLogfilesComponent,
        AdminCompilerMessagesComponent,
        AdminLoadTestComponent,
        MarkdownComponent,
    ],
    providers: [
        HackingService,
        HackingHttpClient,
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
