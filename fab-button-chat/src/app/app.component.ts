import { Component, OnInit } from "@angular/core";
import * as signalR from "@aspnet/signalr";
import {
	animate,
	state,
	style,
	transition,
	trigger,
	query,
	stagger,
	keyframes
} from "@angular/animations";
export const enterLeaveAnimation = [
	trigger("EnterLeave", [
		state("flyIn", style({ transform: "translateY(0)" })),
		transition(":enter", [
			style({ transform: "translateY(200%)" }),
			animate("0.5s 300ms ease-in")
		])
	])
];

@Component({
	selector: "app-root",
	templateUrl: "./app.component.html",
	styleUrls: ["./app.component.scss"],
	animations: enterLeaveAnimation
})
export class AppComponent implements OnInit {
	isButtonActive: boolean;
	title = "fab-button-chat";
	animationState: string;
	ngOnInit(): void {
		const connection = new signalR.HubConnectionBuilder()
			.configureLogging(signalR.LogLevel.Information)
			.withUrl("http://localhost:5000/chathub")
			.build();

		connection
			.start()
			.then(function() {
				console.log("SignalR Connected!");
			})
			.catch(function(err) {
				return console.error(err.toString());
			});

		connection.on("BroadcastMessage", response => {
			console.error(response);
			this.isButtonActive = true;
		});
	}

	onToggleFab(): void {}
}
