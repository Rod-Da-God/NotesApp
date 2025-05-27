
CREATE TABLE "User" (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(10) NOT NULL 
);


CREATE TABLE "Tasks" (
    id SERIAL PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    description TEXT,
    due_date TIMESTAMP NOT NULL,
    status VARCHAR(20) NOT NULL, 
    assigned_user_id INTEGER NOT NULL,
    CONSTRAINT fk_assigned_user 
        FOREIGN KEY (assigned_user_id) 
        REFERENCES "User"(id)
);