$(() => {
    //console.log("hello");
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/taskHub").build();

    connection.start();

    $("#addTask").on('click', function () {
        //console.log("hello");
        const task = $("#assignment").val();
        connection.invoke("NewAssignment", { task} )
    });

    connection.on("NewTask", a => {
        $("#table").append(`<tr id="task-${a.id}"><td>${a.task}</td><td><button class="btn-success accept" data-task-id="${a.id}">I'll take this one</button></td></tr>`)
      
    });

    $("#table").on('click', '.accept', function () {
        const assignment = {
            id: $(this).data('task-id'),
            user: {
                email: $('#userEmail').val()
            }
        };

        connection.invoke('AcceptAssignment', assignment)
    });

    connection.on("AssignmentGotAccepted", assignment => {
        console.log(assignment.user.name);
        $(`#task-${assignment.id}`).find('td:eq(1)').html(`<td><button class="btn-warning" disabled>${assignment.user.name} is doing this</button></td>`);
    });

    connection.on("IAcceptedAssignment", assignment => {
        console.log("hello");
        $(`#task-${assignment.id}`).find('td:eq(1)').html(`<td><button class="btn-warning done" data-task-id="${assignment.Id}">I'm done!</button></td>`);
    });

    $("#table").on('click', '.done', function () {
        const assignment = {
            id: $(this).data('task-id'),
        };

        connection.invoke("FinishedAssignment", assignment)
    });

    connection.on('RemoveFromList', assignment => {
        $(`#task-${assignment.id}`).remove();
    });

});